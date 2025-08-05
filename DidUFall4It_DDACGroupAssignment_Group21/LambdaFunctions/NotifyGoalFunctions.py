import json
import boto3

sns = boto3.client('sns')
SNS_TOPIC_ARN = "arn:aws:sns:us-east-1:600777367894:LearningGoalTopic"

def lambda_handler(event, context):
    print("Received event:", json.dumps(event))

    for record in event['Records']:
        body = record.get('body', '{}')
        try:
            message_wrapper = json.loads(body)
            message_str = message_wrapper.get('Message', body)
            message = json.loads(message_str)
        except json.JSONDecodeError:
            message = {"UserEmail": "unknown@example.com", "Goal": "No goal provided", "EndDate": "No end date", "EventType": "Unknown"}

        user_email = message.get('UserEmail', 'unknown@example.com')
        goal = message.get('Goal', 'No goal provided')
        end_date = message.get('EndDate', 'No end date')
        event_type = message.get('EventType', 'Created')

        if event_type == "Created":
            email_message = f"User {user_email} created a new learning goal: {goal} (Due {end_date})"
        elif event_type == "Completed":
            email_message = f"User {user_email} completed the learning goal: {goal} (Due {end_date})"
        elif event_type == "Deleted":
            email_message = f"User {user_email} deleted the learning goal: {goal} (Due {end_date})"
        else:
            email_message = f"User {user_email} performed an action on the goal: {goal} (Due {end_date})"

        response = sns.publish(
            TopicArn=SNS_TOPIC_ARN,
            Subject=f"Learning Goal {event_type}",
            Message=email_message
        )

        print(f"SNS publish success. MessageId: {response['MessageId']}")

    return {
        "statusCode": 200,
        "body": "SQS messages processed"
    }
    
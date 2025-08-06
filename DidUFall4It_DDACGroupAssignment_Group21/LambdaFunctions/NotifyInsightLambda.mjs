import { SNSClient, PublishCommand } from "@aws-sdk/client-sns";

const snsClient = new SNSClient({});

export const handler = async (event) => {
    try {
        console.log("Received event:", JSON.stringify(event, null, 2));

        const sqsBody = JSON.parse(event.Records[0].body);
        const snsMessage = sqsBody.Message;
        const messageData = JSON.parse(snsMessage);

        const newMessage = {
            type: 'PostProcessingNotification',
            status: 'Insight processed',
            timestamp: new Date().toISOString(),
            originalKey: messageData.key
        };

        const command = new PublishCommand({
            TopicArn: 'arn:aws:sns:us-east-1:067385453713:NewInsightTopic',
            Message: JSON.stringify(newMessage),
        });

        await snsClient.send(command);

        console.log('Published new SNS message:', newMessage);

        return {
            statusCode: 200,
            body: JSON.stringify({ message: 'SNS sent' })
        };
    } catch (error) {
        console.error('Error publishing to SNS:', error);
        return {
            statusCode: 500,
            body: JSON.stringify({ error: error.message })
        };
    }
};
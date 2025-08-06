import { SNSClient, PublishCommand } from "@aws-sdk/client-sns";

const snsClient = new SNSClient({});

export const handler = async (event) => {
    try {
        const snsMessage = event.Records?.[0]?.Sns?.Message;
        const messageData = JSON.parse(snsMessage);

        const newMessage = {
            type: 'PostProcessingNotification',
            status: 'Insight processed',
            timestamp: new Date().toISOString(),
            originalKey: messageData.key
        };

        const command = new PublishCommand({
            TopicArn: 'arn:aws:sns:us-east-1:067385453713:NewInsightTopic', // Replace with your actual topic ARN
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

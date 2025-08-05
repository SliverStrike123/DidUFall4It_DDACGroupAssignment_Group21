import boto3
import base64
import json
import logging
import os
from email.parser import BytesParser
from email.policy import default
import re

logger = logging.getLogger()
logger.setLevel(logging.INFO)

s3 = boto3.client('s3')
sns = boto3.client('sns')

bucket_name = 'didyoufall4it-bucket'
success_topic_arn = 'arn:aws:sns:us-east-1:859425880780:success-image-upload'
invalid_topic_arn = 'arn:aws:sns:us-east-1:859425880780:invalid-image-uploads'
max_size = 5 * 1024 * 1024  # 5 MB
allowed_extensions = ['.png', '.jpg', '.jpeg', '.gif']

def lambda_handler(event, context):
    logger.info("Received event")

    try:
        body = event["body"]
        is_base64_encoded = event["isBase64Encoded"]

        if is_base64_encoded:
            decoded = base64.b64decode(body)
        else:
            decoded = body.encode('utf-8')

        # Extract content-type and boundary
        content_type = event["headers"].get("content-type") or event["headers"].get("Content-Type")
        if not content_type:
            raise ValueError("Missing content type header")

        if content_type.startswith("multipart/form-data"):
            # Extract boundary
            match = re.search(r'boundary="?([^";]+)"?', content_type)
            if not match:
                raise ValueError("Boundary not found in content-type header")

            boundary = match.group(1)
            content = f"Content-Type: {content_type}\r\n\r\n".encode() + decoded
            msg = BytesParser(policy=default).parsebytes(content)

            for part in msg.iter_parts():
                content_disp = part.get("Content-Disposition", "")
                if "filename" in content_disp:
                    filename_match = re.search(r'filename="(.+)"', content_disp)
                    filename = filename_match.group(1) if filename_match else "image.jpg"
                    decoded = part.get_payload(decode=True)
                    content_type = part.get_content_type()
                    break
            else:
                raise ValueError("No file part found in multipart data")

        # File size check
        file_size = len(decoded)
        if file_size > max_size:
            sns.publish(
                TopicArn=invalid_topic_arn,
                Message="Image rejected: exceeds max size",
                Subject="Image Upload Failed"
            )
            logger.info("Unsuccessful Upload, image size too big")
            return {
                "statusCode": 400,
                "body": json.dumps({"error": "File too large. Max 5MB allowed."})
            }

        # Filename and extension check (default fallback if not multipart)
        filename = locals().get("filename") or event.get("queryStringParameters", {}).get("filename", "image.jpg")
        ext = os.path.splitext(filename.lower())[1]
        if ext not in allowed_extensions:
            sns.publish(
                TopicArn=invalid_topic_arn,
                Message=f"Image rejected: unsupported extension {ext}",
                Subject="Image Upload Failed"
            )
            logger.info("Unsuccessful Upload, unsupported extension")
            return {
                "statusCode": 400,
                "body": json.dumps({"error": f"Unsupported file extension: {ext}"})
            }

        # Upload to S3
        key = f"infographics/{filename}"
        s3.put_object(
            Bucket=bucket_name,
            Key=key,
            Body=decoded,
            ContentType=content_type,
            ACL='public-read'
        )
        logger.info(f"Successfully uploaded image to S3 at: https://{bucket_name}.s3.amazonaws.com/{key}")
        sns.publish(
            TopicArn=success_topic_arn,
            Message=f"Image accepted",
            Subject="Infographic Upload Success"
        )

        image_url = f"https://{bucket_name}.s3.amazonaws.com/{key}"
        return {
            "statusCode": 200,
            "body": json.dumps({
                "imageKey": key,
                "imageUrl": image_url
            })
        }

    except Exception as e:
        logger.exception("Unexpected error")
        return {
            "statusCode": 500,
            "body": json.dumps({"error": str(e)})
        }
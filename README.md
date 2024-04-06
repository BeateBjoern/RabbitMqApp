Status: Active development

# RabbitMQ Application
This small project demonstrates RabbitMQ message broker implementation, comprising both a producer and a consumer application.

## Overview:
The producer application, built as a .NET Web API, offers POST and GET endpoints to post messages and get a list of messages back from consumer. It's responsible for publishing messages to the RabbitMQ message broker.

The consumer application is a .NET console application, leveraging hosted services. Its primary task is to consume messages from the RabbitMQ broker. 
Depending on predefined logic, these messages are either requeued, stored in a MongoDB database and appended to a CSV file.

### Data Persistence:
Messages stored in MongoDB for long-term storage.
Appends message data to a CSV file for easy access and analysis.
CSV file accessible to the producer application via a Docker volume.


using System.Configuration;
using System.Messaging;
using VisaNet.Common.MSMQ.MSMQ;

namespace VisaNet.Common.MSMQ
{

    public class VisaNetMSMQ
    {
        private MessageQueue GetMessageQueue()
        {
            var MessageQueueName = ConfigurationManager.AppSettings["NotificationMessageQueueName"];
            if (MessageQueue.Exists(MessageQueueName))
            {
                return new MessageQueue(MessageQueueName) { Label = "VisaNet Pagos Notification Queue" };
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(MessageQueueName);
                return new MessageQueue(MessageQueueName) { Label = "VisaNet Pagos Notification Queue" };
            }
        }

        public Message Receive()
        {
            var messageQueue = GetMessageQueue();
            messageQueue.Formatter = new BinaryMessageFormatter();
            return messageQueue.Receive();
        }

        public void Send(MsmqNotification notification)
        {
            //var messageQueue = GetMessageQueue();
            //messageQueue.Formatter = new BinaryMessageFormatter();
            //messageQueue.Send(notification);
        }
    }
}

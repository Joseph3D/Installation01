using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Game_Logic
{
    /// <summary>
    /// Message Type
    /// </summary>
    public enum AIMessageType
    {
        AIMessage_PointOfInterest = 0,
        AIMessage_EnemyDetected = 1,
        AIMessage_Help = 2,
    }

    /// <summary>
    /// Priority of AI Message
    /// </summary>
    public enum AIMessagePriority
    {
        Priority_Low  = 0,
        Priority_Med  = 1,
        Priority_High = 2,
    }

    /// <summary>
    /// Represents a message from one AIEntity to other AIEntities
    /// </summary>
    public sealed class AIMessage
    {
        /// <summary>
        /// The channel that this message is on.
        /// </summary>
        public byte MessageChannel { get; private set; }

        /// <summary>
        /// The type of this message
        /// </summary>
        public AIMessageType MessageType { get; private set; }

        /// <summary>
        /// The data that this message contains that is relevant to the type of message.
        /// </summary>
        public object MessageData { get; private set; }

        /// <summary>
        /// returns the type of data that this message contains
        /// </summary>
        public Type MessageDataType
        {
            get
            {
                return MessageData.GetType();
            }
        }
    }
}
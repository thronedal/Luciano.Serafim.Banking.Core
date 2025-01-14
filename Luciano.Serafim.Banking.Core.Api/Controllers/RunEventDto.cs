using Luciano.Serafim.Banking.Core.Models.Enums;

namespace Luciano.Serafim.Banking.Core.Api.Controllers
{
    /// <summary>
    /// Event data
    /// </summary>
    public class RunEventDto
    {
        /// <summary>
        /// Event type
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// Origin Account 
        /// required for <see cref="EventType.Transfer"/> and <see cref="EventType.Withdraw"/>
        /// </summary>
        public int? Origin { get; set; }

        /// <summary>
        /// Origin Account 
        /// required for <see cref="EventType.Transfer"/> and <see cref="EventType.Deposit"/>
        /// </summary>
        public int? Destination { get; set; }
        
        /// <summary>
        /// Amount 
        /// </summary>
        public double Amount { get; set; }
    }
    
}

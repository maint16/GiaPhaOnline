using System.Threading.Tasks;
using AppDb.Interfaces;
using Main.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Main.Hubs
{
    [Authorize(PolicyConstant.DefaultSignalRPolicyName)]
    public class NotificationHub : Hub
    {
        #region Properties

        /// <summary>
        /// Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize hub with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public NotificationHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when a client connects to hub.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        #endregion
    }
}
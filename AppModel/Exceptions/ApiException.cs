﻿using System;
using System.Net;

namespace AppModel.Exceptions
{
    public class ApiException : Exception
    {
        #region Properties

        public HttpStatusCode Status { get; set; }

        #endregion

        #region Constructor

        public ApiException()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApiException(string message, HttpStatusCode status) : base(message)
        {
            Status = status;
        }

        #endregion
    }
}
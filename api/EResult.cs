using System;

namespace api
{
    public class EResult
    {
        public object Data;
        public string ErrorMessage;
        public Exception Exception;

        public bool Success;

        public EResult()
        {
        }

        /// <summary>
        ///     No data returned
        /// </summary>
        /// <param name="success"></param>
        public EResult(bool success) : this()
        {
            Success = success;
        }

        /// <summary>
        ///     Data returned successfully
        /// </summary>
        /// <param name="data"></param>
        public EResult(object data) : this()
        {
            Success = true;
            Data = data;
        }

        /// <summary>
        ///     Error occured with no more information than an error message
        /// </summary>
        /// <param name="errorMessage"></param>
        public EResult(string errorMessage) : this()
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Error with both a message and data: e.g. the id passed
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="data"></param>
        public EResult(string errorMessage, object data) : this(errorMessage)
        {
            Data = data;
        }

        /// <summary>
        ///     Error where DB returned something, but input is not returned: DO NOT use when there is input, only on methods where
        ///     there is none
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="dBException"></param>
        public EResult(string errorMessage, Exception dBException) : this(errorMessage)
        {
            Exception = dBException;
        }

        /// <summary>
        ///     A DB error with lots of info about what went wrong.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="data"></param>
        /// <param name="exception"></param>
        public EResult(string errorMessage, object data, Exception exception) : this(errorMessage)
        {
            Data = data;
            Exception = exception;
        }
    }
}
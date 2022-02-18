/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.AspNetCore.Mvc;

namespace DSynth.Models
{
    public class EndpointResponse : ControllerBase
    {
        public int StatCode { get; set; }
        public string Message { get; set; }
        public ObjectResult Resp => BuildResp();

        public EndpointResponse(int statCode, string message)
        {
            StatCode = statCode;
            Message = message;
        }

        public static EndpointResponse CreateNew(int statusCode, string message)
        {
            return new EndpointResponse(statusCode, message);
        }

        private ObjectResult BuildResp()
        {
            object endpointStatus = new
            {
                statusCode = StatCode,
                message = Message
            };

            return StatusCode(StatCode, endpointStatus);
        }
    }
}
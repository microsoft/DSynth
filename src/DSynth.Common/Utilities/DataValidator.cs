/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DSynth.Common.Utilities
{
    public static class DataValidator
    {
        ///<Summary>
        /// This method validates configuration classes to make sure they have expected values
        ///<Summary>
        public static void Validate(object itemToValidate)
        {
            ConcurrentBag<ValidationException> validationExceptions = new ConcurrentBag<ValidationException>();
            var context = new ValidationContext(itemToValidate, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(itemToValidate, context, validationResults, true);
            if (validationResults.Count > 0)
            {
                StringBuilder concatenatedErrors = new StringBuilder("Item: ").Append(itemToValidate.ToString()).Append(" ==> ");
                foreach (var res in validationResults)
                {
                    concatenatedErrors.Append(res.ErrorMessage).Append(";");
                }

                validationExceptions.Add(new ValidationException(concatenatedErrors.ToString()));
            }


            if (validationExceptions.Count > 0)
            {
                throw new AggregateException(validationExceptions);
            }
        }
    }
}
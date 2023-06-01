// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class CommaSeparatedArrayBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            string[] model =
                valueProviderResult
                    .Values
                    .SelectMany(value =>
                        value?.Split(",", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
                    .ToArray();

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
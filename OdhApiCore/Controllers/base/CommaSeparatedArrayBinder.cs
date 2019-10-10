using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class CommaSeparatedArrayBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.Result = ModelBindingResult.Success(new string[] { });
            var value = valueProviderResult.FirstValue; // get the value as string

            var model = value?.Split(",") ?? new string[] { };
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
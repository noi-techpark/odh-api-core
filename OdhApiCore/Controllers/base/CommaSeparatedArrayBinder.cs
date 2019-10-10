using System.Threading.Tasks;

namespace OdhApiCore.Controllers
{
    public class CommaSeparatedArrayBinder : Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder
    {
        public Task BindModelAsync(Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var value = valueProviderResult.FirstValue; // get the value as string

            var model = value.Split(",");
            bindingContext.Result = Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
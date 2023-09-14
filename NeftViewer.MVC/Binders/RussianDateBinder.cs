using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace NeftViewer.MVC.Binders
{
    public class RussianDateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult != ValueProviderResult.None && DateTime.TryParseExact(valueProviderResult.FirstValue, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                bindingContext.Result = ModelBindingResult.Success(date);
                return Task.CompletedTask;
            }
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }

}

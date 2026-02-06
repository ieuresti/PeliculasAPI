using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace PeliculasAPI.Utilidades
{
    // Binder personalizado que deserializa una cadena JSON proporcionada por el ValueProvider al tipo CLR esperado por el modelo.
    // Útil cuando se envían objetos complejos en parámetros donde el binder por defecto no aplica.
    public class TypeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Nombre del parámetro o propiedad que estamos intentando enlazar
            var nombrePropiedad = bindingContext.ModelName;

            // Intenta obtener el valor asociado a ese nombre desde los ValueProviders (query, form, route, etc.)
            var valor = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            // Si no hay valor proporcionado, no hacemos nada y permitimos que otros binders actúen
            if (valor == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                // Tipo CLR que se espera para el modelo (p. ej. MiFiltroDTO, List<int>, etc.)
                var tipoDestino = bindingContext.ModelMetadata.ModelType;

                // Deserializamos la primera cadena encontrada al tipo destino.
                // Se usa PropertyNameCaseInsensitive para aceptar diferentes mayúsculas/minúsculas
                var valorDeserializado = JsonSerializer.Deserialize(valor.FirstValue!,
                    tipoDestino, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Si la deserialización fue correcta, marcamos el binding como exitoso
                bindingContext.Result = ModelBindingResult.Success(valorDeserializado);
            } catch
            {
                // Si ocurre cualquier error, añadimos un error de modelo para que el pipeline pueda informar
                bindingContext.ModelState.TryAddModelError(nombrePropiedad, "El valor proporcionado no es válido para el tipo esperado.");
            }

            // Terminamos (la operación es efectivamente síncrona, por eso devolvemos Task.CompletedTask)
            return Task.CompletedTask;
        }
    }
}

using System.Globalization; // Необходимо для CultureInfo
using System.Windows.Controls; // Необходимо для ValidationRule и ValidationResult

// Убедитесь, что это пространство имен соответствует тому,
// которое вы используете для префикса 'local' в вашем XAML
// (например, xmlns:local="clr-namespace:ClientAddressManager")
namespace taskBD
{
    /// <summary>
    /// Правило валидации, проверяющее, что строковое поле не является пустым или состоящим только из пробелов.
    /// </summary>
    public class RequiredFieldRule : ValidationRule
    {
        /// <summary>
        /// Получает или задает имя поля, которое будет отображаться в сообщении об ошибке.
        /// Это свойство можно установить в XAML.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Выполняет проверку валидности значения.
        /// </summary>
        /// <param name="value">Значение, которое нужно проверить.</param>
        /// <param name="cultureInfo">Информация о культуре для проверки.</param>
        /// <returns>Результат валидации.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Пытаемся преобразовать значение в строку
            string inputString = value as string;

            // Проверяем, является ли строка null, пустой или состоящей только из пробельных символов
            if (string.IsNullOrWhiteSpace(inputString))
            {
                // Формируем сообщение об ошибке, используя FieldName если оно задано,
                // иначе используем общее сообщение "Поле".
                string errorMessage = string.IsNullOrWhiteSpace(FieldName)
                                      ? "Это поле не может быть пустым."
                                      : $"{FieldName} не может быть пустым.";
                return new ValidationResult(false, errorMessage);
            }

            // Если строка не пустая, валидация пройдена успешно
            return ValidationResult.ValidResult;
        }
    }
}
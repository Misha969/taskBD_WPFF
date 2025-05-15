using System.Globalization; // Для CultureInfo
using System.Text.RegularExpressions; // Для регулярных выражений
using System.Windows.Controls; // Для ValidationRule

namespace taskBD // Убедитесь, что это ваше корректное пространство имен
{
    public class EmailValidationRule : ValidationRule
    {
        // Переопределяем метод Validate, который будет выполнять проверку
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string email = value as string;

            if (string.IsNullOrWhiteSpace(email))
            {
                // Можно сделать Email необязательным, тогда эта проверка не нужна
                // или вернуть return ValidationResult.ValidResult; если пустое значение допустимо.
                // В данном примере будем считать, что если поле заполнено, то оно должно быть валидным email.
                // Если Email обязателен, то эта проверка должна возвращать ошибку:
                // return new ValidationResult(false, "Email не может быть пустым.");
                return ValidationResult.ValidResult; // Предполагаем, что пустой Email допустим (необязательное поле)
            }

            // Простое регулярное выражение для проверки формата Email
            // Для более строгой валидации можно использовать более сложное выражение
            // или сторонние библиотеки.
            string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                return new ValidationResult(false, "Некорректный формат Email адреса.");
            }

            // Если все проверки пройдены, возвращаем результат валидности
            return ValidationResult.ValidResult;
        }
    }
}
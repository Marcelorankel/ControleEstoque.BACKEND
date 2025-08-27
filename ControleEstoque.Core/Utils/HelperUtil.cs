namespace ControleEstoque.Core.Utils
{
    public static class HelperUtil
    {
        public static int ObterValorNumericoEnum<TEnum>(string nomeEnum) where TEnum : struct, Enum
        {
            if (Enum.TryParse<TEnum>(nomeEnum, ignoreCase: true, out var enumValue))
            {
                return Convert.ToInt32(enumValue);
            }
            else
            {
                throw new ArgumentException($"O valor '{nomeEnum}' não é válido para o enum {typeof(TEnum).Name}");
            }
        }
    }
}
using AlunosTech.Domain.Exceptions;

namespace AlunosTech.Domain.Common;

public static class Guard
{
    public static string Obrigatorio(string? valor, int tamanhoMaximo, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DomainException($"{campo} é obrigatório.");
        }

        var texto = valor.Trim();

        if (texto.Length > tamanhoMaximo)
        {
            throw new DomainException($"{campo} deve ter no máximo {tamanhoMaximo} caracteres.");
        }

        return texto;
    }

    public static string? Opcional(string? valor, int tamanhoMaximo, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        return Obrigatorio(valor, tamanhoMaximo, campo);
    }

    public static Guid Obrigatorio(Guid valor, string campo)
    {
        if (valor == Guid.Empty)
        {
            throw new DomainException($"{campo} é obrigatório.");
        }

        return valor;
    }

    public static int Positivo(int valor, string campo)
    {
        if (valor <= 0)
        {
            throw new DomainException($"{campo} deve ser maior que zero.");
        }

        return valor;
    }
}

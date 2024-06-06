using System;

public class Complex
{

    public static readonly Complex Zero = new(0, 0);
    public static readonly Complex One = new(1, 0);
    public static readonly Complex ImaginaryOne = new(0, 1);
    public double X { get; init; }
    public double Y { get; init; }

    public Complex(double x, double y)

    {
        X = x;
        Y = y;
    }
    public Complex(double x) : this(x, 0) { }
    public Complex() : this(0, 0) { }
    public static Complex Re(double x) => new(x, 0);
    public static Complex Im(double y) => new(0, y);
    public static Complex Sqrt(double val)
    {

        if (val < 0)
        {

            return new Complex(0, Math.Sqrt(-val));
        }
        else

        {
            return new Complex(Math.Sqrt(val), 0);
        }
    }
    public double Length => Math.Sqrt(X * X + Y * Y);
    public static Complex operator +(Complex a, Complex b)
    {
        return new Complex(a.X + b.X, a.Y + b.Y);
    }
    public static Complex operator -(Complex a, Complex b)
    {
        return new Complex(a.X - b.X, a.Y - b.Y);
    }
    public static Complex operator *(Complex a, Complex b)
    {
        return new Complex(
            a.X * b.X - a.Y * b.Y,
            a.X * b.Y + a.Y * b.X
        );
    }

    public static Complex operator /(Complex a, Complex b)
    {
        if (b.Equals(Zero))
        {
            throw new DivideByZeroException("Нельзя делить на ноль");
        }

        double denom = b.X * b.X + b.Y * b.Y;
        return new Complex(
            (a.X * b.X + a.Y * b.Y) / denom,
            (a.Y * b.X - a.X * b.Y) / denom
        );
    }
    public static Complex operator +(Complex a) => a;
    public static Complex operator -(Complex a)
    {
        return new Complex(-a.X, -a.Y);
    }
    public override string ToString()
    {
        return $"{X} + {Y}i";
    }
    public override bool Equals(object obj)
    {
        if (obj is Complex o)
        {
            return X == o.X && Y == o.Y;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
public interface IEquation
{
    int Size { get; }
    double[] Coeffs { get; }
    Complex[] Solve();
}
public class Equation : IEquation
{
    private double[] _coeffs;
    private IRootStrategy _strategy;

    public int Size => _coeffs.Length;
    public double[] Coeffs => (double[])_coeffs.Clone();
    public Equation(double[] coeffs)
    {
        _coeffs = Utils.TrimZeros(coeffs);
        _strategy = Utils.GetStrategy(_coeffs);
    }

    public Complex[] Solve() => _strategy.Solve(_coeffs);
}
public static class Utils
{
    public static double[] TrimZeros(double[] coeffs)
    {
        int i = 0;
        while (i < coeffs.Length && coeffs[i] == 0) i++;
        double[] result = new double[coeffs.Length - i];
        Array.Copy(coeffs, i, result, 0, result.Length);
        return result;
    }
    public static IRootStrategy GetStrategy(double[] coeffs)
    {
        return coeffs.Length switch
        {
            2 => Strategies.Linear,
            3 => Strategies.Quadratic,
            _ => throw new InvalidOperationException("Ошибка")
        };
    }
    public static Equation CreateEquation(double[] coeffs)
    {
        double[] trimmed = TrimZeros(coeffs);
        IRootStrategy strategy = GetStrategy(trimmed);
        return new Equation(trimmed);
    }
}
public interface IRootStrategy
{
    Complex[] Solve(double[] coeffs);
}
public static class Strategies
{
    public static readonly IRootStrategy Linear = new LinearStrategy();
    public static readonly IRootStrategy Quadratic = new QuadraticStrategy();
    private class LinearStrategy : IRootStrategy
    {
        public Complex[] Solve(double[] coeffs)
        {
            if (coeffs.Length < 2) throw new InvalidOperationException("Недостаточно коэфф для линейного уравнения");
            double a = coeffs[0], b = coeffs[1];

            if (a == 0) throw b == 0 ? new InvalidOperationException("Бесконечность") : new InvalidOperationException("Нет корней");

            return new[] { new Complex(-b / a, 0) };
        }
    }
    private class QuadraticStrategy : IRootStrategy
    {
        public Complex[] Solve(double[] coeffs)
        {
            if (coeffs.Length < 3) throw new InvalidOperationException("Недостаточно коэфф для квадратного уравнения");
            double a = coeffs[0], b = coeffs[1], c = coeffs[2];
            if (a == 0) return Strategies.Linear.Solve(new[] { b, c });

            double d = b * b - 4 * a * c;
            if (d < 0)
            {
                Complex sqrtD = Complex.Sqrt(-d);
                return new[]
                {
                    new Complex((-b) / (2 * a), sqrtD.Y / (2 * a)),
                    new Complex(-b / (2 * a), -sqrtD.Y / (2 * a))
                };
            }
            else
            {
                Complex sqrtD = Complex.Sqrt(d);
                return new[]
                {
                    new Complex((-b + sqrtD.X) / (2 * a), 0),
                    new Complex((-b - sqrtD.X) / (2 * a), 0)
                };
            }
        }
    }
}
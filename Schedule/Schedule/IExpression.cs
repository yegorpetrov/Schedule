
namespace Schedule
{
    public interface IExpression<T>
    {
        /// <summary>
        /// Find the nearest value or minimum/maximum (depending on direction) if not found
        /// </summary>
        /// <param name="base">The value to start looking from</param>
        /// <param name="out">The output</param>
        /// <param name="forward">The direction to look in</param>
        /// <returns>True if there's a value ahead/behind of the base value</returns>
        bool Find(T @base, ref T @out, bool forward = true);
    }
}

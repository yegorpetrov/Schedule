using Schedule;
using Schedule.Expressions;
using Schedule.Parser;
using System;

namespace Test
{

	/// <summary>
	/// Класс для задания и расчета времени по расписанию.
	/// </summary>
	public class Schedule
	{
        private readonly ScheduleExpression _schedule;
        private readonly string _scheduleString;

        /// <summary>
        /// Создает пустой экземпляр, который будет соответствовать
        /// расписанию типа "*.*.* * *:*:*.*" (раз в 1 мс).
        /// </summary>
        public Schedule() : this("*.*.* * *:*:*.*")
		{
		}

		/// <summary>
		/// Создает экземпляр из строки с представлением расписания.
		/// </summary>
		/// <param name="scheduleString">Строка расписания.
		/// Формат строки:
		///     yyyy.MM.dd w HH:mm:ss.fff
		///     yyyy.MM.dd HH:mm:ss.fff
		///     HH:mm:ss.fff
		///     yyyy.MM.dd w HH:mm:ss
		///     yyyy.MM.dd HH:mm:ss
		///     HH:mm:ss
		/// Где yyyy - год (2000-2100)
		///     MM - месяц (1-12)
		///     dd - число месяца (1-31 или 32). 32 означает последнее число месяца
		///     w - день недели (0-6). 0 - воскресенье, 6 - суббота
		///     HH - часы (0-23)
		///     mm - минуты (0-59)
		///     ss - секунды (0-59)
		///     fff - миллисекунды (0-999). Если не указаны, то 0
		/// Каждую часть даты/времени можно задавать в виде списков и диапазонов.
		/// Например:
		///     1,2,3-5,10-20/3
		///     означает список 1,2,3,4,5,10,13,16,19
		/// Дробью задается шаг в списке.
		/// Звездочка означает любое возможное значение.
		/// Например (для часов):
		///     */4
		///     означает 0,4,8,12,16,20
		/// Вместо списка чисел месяца можно указать 32. Это означает последнее
		/// число любого месяца.
		/// Пример:
		///     *.9.*/2 1-5 10:00:00.000
		///     означает 10:00 во все дни с пн. по пт. по нечетным числам в сентябре
		///     *:00:00
		///     означает начало любого часа
		///     *.*.01 01:30:00
		///     означает 01:30 по первым числам каждого месяца
		/// </param>
		public Schedule(string scheduleString)
		{
            _schedule = new ScheduleParser().ParseSchedule(scheduleString);
            _scheduleString = scheduleString;
        }

		/// <summary>
		/// Возвращает следующий ближайший к заданному времени момент в расписании или
		/// само заданное время, если оно есть в расписании.
		/// </summary>
		/// <param name="t1">Заданное время</param>
		/// <returns>Ближайший момент времени в расписании</returns>
		public DateTime NearestEvent(DateTime t1)
		{
            DateTime result = default;

            if (_schedule.Find(t1, ref result, true))
            {
                return result;
            }
            else
            {
                throw new NoScheduleContinutationFoundException(_scheduleString);
            }
		}

		/// <summary>
		/// Возвращает предыдущий ближайший к заданному времени момент в расписании или
		/// само заданное время, если оно есть в расписании.
		/// </summary>
		/// <param name="t1">Заданное время</param>
		/// <returns>Ближайший момент времени в расписании</returns>
		public DateTime NearestPrevEvent(DateTime t1)
		{
            DateTime result = default;

            if (_schedule.Find(t1, ref result, false))
            {
                return result;
            }
            else
            {
                throw new NoScheduleContinutationFoundException(_scheduleString);
            }
        }

        /// <summary>
        /// Возвращает следующий момент времени в расписании.
        /// </summary>
        /// <param name="t1">Время, от которого нужно отступить</param>
        /// <returns>Следующий момент времени в расписании</returns>
        public DateTime NextEvent(DateTime t1)
        {
            return NearestEvent(t1.AddMilliseconds(1));
        }

		/// <summary>
		/// Возвращает предыдущий момент времени в расписании.
		/// </summary>
		/// <param name="t1">Время, от которого нужно отступить</param>
		/// <returns>Предыдущий момент времени в расписании</returns>
		public DateTime PrevEvent(DateTime t1)
		{
            return NearestPrevEvent(t1.AddMilliseconds(-1));
        }

	}
	
}
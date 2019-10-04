using System;

namespace Test
{

	/// <summary>
	/// ����� ��� ������� � ������� ������� �� ����������.
	/// </summary>
	public class Schedule
	{

		/// <summary>
		/// ������� ������ ���������, ������� ����� ���������������
		/// ���������� ���� "*.*.* * *:*:*.*" (��� � 1 ��).
		/// </summary>
		public Schedule()
		{
		}

		/// <summary>
		/// ������� ��������� �� ������ � �������������� ����������.
		/// </summary>
		/// <param name="scheduleString">������ ����������.
		/// ������ ������:
		///     yyyy.MM.dd w HH:mm:ss.fff
		///     yyyy.MM.dd HH:mm:ss.fff
		///     HH:mm:ss.fff
		///     yyyy.MM.dd w HH:mm:ss
		///     yyyy.MM.dd HH:mm:ss
		///     HH:mm:ss
		/// ��� yyyy - ��� (2000-2100)
		///     MM - ����� (1-12)
		///     dd - ����� ������ (1-31 ��� 32). 32 �������� ��������� ����� ������
		///     w - ���� ������ (0-6). 0 - �����������, 6 - �������
		///     HH - ���� (0-23)
		///     mm - ������ (0-59)
		///     ss - ������� (0-59)
		///     fff - ������������ (0-999). ���� �� �������, �� 0
		/// ������ ����� ����/������� ����� �������� � ���� ������� � ����������.
		/// ��������:
		///     1,2,3-5,10-20/3
		///     �������� ������ 1,2,3,4,5,10,13,16,19
		/// ������ �������� ��� � ������.
		/// ��������� �������� ����� ��������� ��������.
		/// �������� (��� �����):
		///     */4
		///     �������� 0,4,8,12,16,20
		/// ������ ������ ����� ������ ����� ������� 32. ��� �������� ���������
		/// ����� ������ ������.
		/// ������:
		///     *.9.*/2 1-5 10:00:00.000
		///     �������� 10:00 �� ��� ��� � ��. �� ��. �� �������� ������ � ��������
		///     *:00:00
		///     �������� ������ ������ ����
		///     *.*.01 01:30:00
		///     �������� 01:30 �� ������ ������ ������� ������
		/// </param>
		public Schedule(string scheduleString)
		{
		}

		/// <summary>
		/// ���������� ��������� ��������� � ��������� ������� ������ � ���������� ���
		/// ���� �������� �����, ���� ��� ���� � ����������.
		/// </summary>
		/// <param name="t1">�������� �����</param>
		/// <returns>��������� ������ ������� � ����������</returns>
		public DateTime NearestEvent(DateTime t1)
		{
		}

		/// <summary>
		/// ���������� ���������� ��������� � ��������� ������� ������ � ���������� ���
		/// ���� �������� �����, ���� ��� ���� � ����������.
		/// </summary>
		/// <param name="t1">�������� �����</param>
		/// <returns>��������� ������ ������� � ����������</returns>
		public DateTime NearestPrevEvent(DateTime t1)
		{
		}

		/// <summary>
		/// ���������� ��������� ������ ������� � ����������.
		/// </summary>
		/// <param name="t1">�����, �� �������� ����� ���������</param>
		/// <returns>��������� ������ ������� � ����������</returns>
		public DateTime NextEvent(DateTime t1)
		{
		}

		/// <summary>
		/// ���������� ���������� ������ ������� � ����������.
		/// </summary>
		/// <param name="t1">�����, �� �������� ����� ���������</param>
		/// <returns>���������� ������ ������� � ����������</returns>
		public DateTime PrevEvent(DateTime t1)
		{
		}

	}
	
}
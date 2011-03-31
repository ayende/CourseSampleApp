using System;

namespace CourseSampleApp.Models
{
	public class BookLoan
	{
		public virtual int Id { get; set; }
		public virtual Member Member { get; set; }
		public virtual Book Book { get; set; }
		public virtual LendingPeriod LendingPeriod { get; set; }
	}

	public class LendingPeriod
	{
		public virtual DateTime DueDate { get; set; }
		public virtual DateTime LoanDate { get; set; }

		public virtual bool IsLate
		{
			get { return DateTime.Today > DueDate; }
		}
	}
}
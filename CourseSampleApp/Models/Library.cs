using System.Collections.Generic;

namespace CourseSampleApp.Models
{
	public class Library
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<Member> Members { get; set; }
		public virtual ICollection<Book> Books { get; set; }
	}
}
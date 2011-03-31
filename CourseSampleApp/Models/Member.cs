namespace CourseSampleApp.Models
{
	public class Member
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Library Library { get; set; }
	}
}
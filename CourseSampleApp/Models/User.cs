using System;
using Iesi.Collections.Generic;

namespace CourseSampleApp.Models
{
	public class User
	{
		public User()
		{
			Blogs = new HashedSet<Blog>();
		}

		public virtual ISet<Blog> Blogs { get; set; }


		public virtual int Id { get; set; }

		public virtual byte[] Password { get; set; }

		public virtual string Email { get; set; }

		public virtual string Username { get; set; }

		public virtual DateTime CreatedAt { get; set; }

		public virtual string Bio { get; set; }
	}
}
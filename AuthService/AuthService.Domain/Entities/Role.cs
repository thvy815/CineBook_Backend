using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
	[Table("roles")]
	public class Role
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Required, MaxLength(30)]
		[Column("name")]
		public string Name { get; set; }

		[Column("description")]
		public string Description { get; set; }

		public ICollection<User> Users { get; set; }
	}
}

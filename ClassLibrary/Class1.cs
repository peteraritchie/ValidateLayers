using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRI
{
	namespace Data
	{
		using FrontEnd;
		public class Entity
		{
			public View FontEnd { get; set; }
		}

	}

	namespace Domain
	{
		using Data;

		public class EntityRepository
		{
		}

		public class Entity
		{
		}
	}

	namespace ViewModels
	{
		public class ViewModel
		{
			private Data.Entity dataEntity;
		}
	}

	namespace FrontEnd
	{
		using Domain;

		public class View
		{
			private Data.Entity dataEntity;
			private ViewModels.ViewModel viewModel;
		}
	}
}

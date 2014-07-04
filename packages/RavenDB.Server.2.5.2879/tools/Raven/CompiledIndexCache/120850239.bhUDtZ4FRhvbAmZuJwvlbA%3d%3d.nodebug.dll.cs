using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;


public class Transformer_Customers_2fMyTras_a2 : Raven.Database.Linq.AbstractTransformer
{
	public Transformer_Customers_2fMyTras_a2()
	{
		this.ViewText = @"from customer in results select new { customer.Id, customer.Name };
";
		this.TransformResultsDefinition = results => 
			from customer in results
			select new {
				customer.Id,
				customer.Name
			};
	}
}

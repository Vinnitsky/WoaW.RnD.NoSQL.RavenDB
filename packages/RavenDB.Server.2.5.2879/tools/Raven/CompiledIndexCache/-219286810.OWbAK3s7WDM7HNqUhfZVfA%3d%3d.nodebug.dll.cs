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


public class Transformer_IdAndNamesOnly : Raven.Database.Linq.AbstractTransformer
{
	public Transformer_IdAndNamesOnly()
	{
		this.ViewText = @"results.Select(customer => new {
    Id = customer.Id,
    Name = customer.Name
})
";
		this.TransformResultsDefinition = results => results.Select((Func<dynamic, dynamic>)(customer => new {
			Id = customer.Id,
			Name = customer.Name
		}));
	}
}

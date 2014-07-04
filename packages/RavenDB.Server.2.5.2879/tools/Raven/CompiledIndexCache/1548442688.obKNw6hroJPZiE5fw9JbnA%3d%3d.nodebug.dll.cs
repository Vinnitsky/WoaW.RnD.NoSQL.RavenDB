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


public class Transformer_aa : Raven.Database.Linq.AbstractTransformer
{
	public Transformer_aa()
	{
		this.ViewText = @"from doc in docs.Entity1s
select new { Attributes_a2 = doc.Attributes.a2 }
";
		this.TransformResultsDefinition = results => 
			from doc in docs.Entity1s
			select new {
				Attributes_a2 = doc.Attributes.a2
			};
	}
}

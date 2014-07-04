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


public class Index_Auto_2fEntity1s_2fByAttributes_a2 : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fEntity1s_2fByAttributes_a2()
	{
		this.ViewText = @"from doc in docs.Entity1s
select new { Attributes_a2 = doc.Attributes.a2 }";
		this.ForEntityNames.Add("Entity1s");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Entity1s", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Attributes_a2 = doc.Attributes.a2,
				__document_id = doc.__document_id
			});
		this.AddField("Attributes_a2");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Attributes.a2");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Attributes.a2");
		this.AddQueryParameterForReduce("__document_id");
	}
}

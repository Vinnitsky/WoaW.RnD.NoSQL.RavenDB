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


public class Index_DeleteAllCustomers : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_DeleteAllCustomers()
	{
		this.ViewText = @"docs.Customers.Select(entity => new {})";
		this.ForEntityNames.Add("Customers");
		this.AddMapDefinition(docs => docs.Where(__document => string.Equals(__document["@metadata"]["Raven-Entity-Name"], "Customers", System.StringComparison.InvariantCultureIgnoreCase)).Select((Func<dynamic, dynamic>)(entity => new {
			__document_id = entity.__document_id
		})));
		this.AddField("__document_id");
	}
}

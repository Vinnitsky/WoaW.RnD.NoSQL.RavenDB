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


public class Index_Customers_2fMyIndex : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Customers_2fMyIndex()
	{
		this.ViewText = @"from customer in docs.Customers select new { customer.Id, customer.Name }";
		this.ForEntityNames.Add("Customers");
		this.AddMapDefinition(docs => 
			from customer in docs
			where string.Equals(customer["@metadata"]["Raven-Entity-Name"], "Customers", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				customer.Id,
				customer.Name,
				__document_id = customer.__document_id
			});
		this.AddField("__document_id");
		this.AddField("Id");
		this.AddField("Name");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForMap("Id");
		this.AddQueryParameterForMap("Name");
		this.AddQueryParameterForReduce("__document_id");
		this.AddQueryParameterForReduce("Id");
		this.AddQueryParameterForReduce("Name");
	}
}

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


public class Index_Customers_2fMyTras_a2 : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Customers_2fMyTras_a2()
	{
		this.ViewText = @"from order in docs.Orders select new {order.Id, order.Name}";
		this.ForEntityNames.Add("Orders");
		this.AddMapDefinition(docs => 
			from order in docs
			where string.Equals(order["@metadata"]["Raven-Entity-Name"], "Orders", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				order.Id,
				order.Name,
				__document_id = order.__document_id
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

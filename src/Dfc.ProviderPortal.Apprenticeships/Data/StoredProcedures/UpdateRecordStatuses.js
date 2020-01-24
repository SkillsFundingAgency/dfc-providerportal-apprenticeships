﻿
function updateSproc(UKPRN, currentStatus, statusToBeChangedTo) {
    var collection = getContext().getCollection();
  

    var responseBody = {
        updated: 0,
        continuation: true,
        error: "",
        log: ""
    };

    if (typeof currentStatus !== 'number') throw new Error('currentStatus must be a number');
    if (typeof statusToBeChangedTo !== 'number') throw new Error('statusToBeChangedTo must be a number');
    if (typeof UKPRN !== 'number') throw new Error('UKPRN must be a number');

    var updated = 0;

    tryQueryAndUpdate();

    // Recursively queries for a document by id w/ support for continuation tokens.
    // Calls tryUpdate(document) as soon as the query returns a document.
    function tryQueryAndUpdate(continuation) {
        var query = {
            query: "select * from apprenticeship apr where apr.RecordStatus = @currentStatus and apr.ProviderUKPRN = @UKPRN", parameters: [{ name: "@currentStatus", value: currentStatus },
            { name: "@UKPRN", value: UKPRN }]
        };


        var requestOptions = { continuation: continuation };
   
            if (documents.length > 0) {
                responseBody.log += "Found documents: " + documents.length;
                // If the document is found, update it.
                // There is no need to check for a continuation token since we are querying for a single document.
                for (var i = 0; i < documents.length; i++)
                    tryUpdate(documents[i]);
                tryQueryAndUpdate(responseOptions.continuation);
            } else if (responseOptions.continuation) {
                // Else if the query came back empty, but with a continuation token; repeat the query w/ the token.
                // It is highly unlikely for this to happen when performing a query by id; but is included to serve as an example for larger queries.
                responseBody.log += "Continue query";
                tryQueryAndUpdate(responseOptions.continuation);
            } else
            {              
                response.setBody({ updated: updated });
            }
        });
        // If we hit execution bounds - throw an exception.
        // This is highly unlikely given that this is a query by id; but is included to serve as an example for larger queries.
        if (!isAccepted) {
            throw new Error("The stored procedure timed out.");
        }
    }
    // Updates the supplied document according to the update object passed in to the sproc.
    function tryUpdate(document) {
        // DocumentDB supports optimistic concurrency control via HTTP ETag.
        var requestOptions = { etag: document._etag };
        document.RecordStatus = statusToBeChangedTo;
        // Update the document.
        var isAccepted = collection.replaceDocument(document._self, document, requestOptions, function (err, updatedDocument, responseOptions) {
            if (err) throw err;
            // If we have successfully updated the document - return it in the response body.
            updated++;
        });
        // If we hit execution bounds - throw an exception.
        if (!isAccepted) {
            throw new Error("The stored procedure timed out.");
        }
    }
}
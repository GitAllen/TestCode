$.post('/MonetaryCredit', function (data) {
    var table = $('<table />')
       .attr('style', 'border: 1px solid black; border-collapse: collapse; width: 100%')
       .append($('<thead />').attr('style', 'font-weight: bold')
       .append($('<td>Subscription</td><td>Remaining Credit</td><td>Remaining days</td><td>Burn Rate</td><td>Might Run Out</td>')));

    $.each(data, function (idx, sub) {
        console.log(sub);
        console.log(sub.SubscriptionName);
        console.log(sub.RemainingCreditDisplay);
        console.log(sub.DaysLeftInBillingPeriod);
        console.log(sub.BurnRateDisplay);
        console.log(sub.MightRunOut);
        if (sub != null) {
            table.append($('<tr />')
                  .attr('style', 'border: 1px solid black;')
                  .append($('<td />').text(sub.SubscriptionName))
                  .append($('<td />').text(sub.RemainingCreditDisplay))
                  .append($('<td />').text(sub.DaysLeftInBillingPeriod))
                  .append($('<td />').text(sub.BurnRateDisplay))
                  .append($('<td />').text(sub.MightRunOut)));
        }
    });

    $('<div title="My Subscriptions - Credit" />')
        .append(table)
        .dialog({ width: 600, height: 250 });
});
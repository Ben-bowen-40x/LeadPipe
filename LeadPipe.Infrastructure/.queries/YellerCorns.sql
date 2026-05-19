/*Yeller Corns*/
select
	f.phonenumber as `Phone Number`,
	f.date as `Date of Message`,
	f.payload as `Message Contents`,
	f.metadata as `Message Source`,
	if(f.unixdate < s.unixdate and f.unixdate < c.unixdate, 1, 0) as `IM Lead`,
	1 as `Potential Sales Lead`,
	c.id as `Customer ID`, 
    s.active as `Subscription is Active`, 
    c.date as `Customer record start date`, 
    c.unixcanceldate as `Customer unix cxl date`,
    s.id as `Subscription Id`, 
    s.complete as `Completed Initial`, 
    s.value as `Contract Value`, 
    s.date as `Subscription Start Date`, 
    s.unixcanceldate as `Subscription unix cxl date`,
    s.type as `Service Type`,

 /*For debugging*/
    f.id as `Corn Id`

from cornentities f 
left join custardentities c on f.phonenumber in (c.phonenumber, c.phonenumber2)
left join sandentities s on s.custardid = c.id
where f.phonenumber > 0 and f.source = 'Sandbox';

﻿delete from r_wms_location where ROW < '05' and BAY in ('001','002') and LEVEL < '04';
delete from r_wms_location where ROW in ('02','04','06','08','10','12') and BAY in ('002','003') and LEVEL in ('06','07');
delete from r_wms_location where ROW in ('02','04','06','08','10','12') and BAY = '003' and LEVEL = '03';
delete from r_wms_location where ROW in ('02','04') and BAY in ('004','027','049','071') and LEVEL < '08';
delete from r_wms_location where ROW in ('02','04') and BAY in ('025','026','047','048','069','070') and LEVEL in ('06','07');
delete from r_wms_location where ROW in ('05','06','07','08','09','10','11','12') and BAY in ('001','002') and LEVEL < '04';
delete from r_wms_location where ROW in ('06','08','10','12') and BAY in ('004','027','049','073') and LEVEL < '08';
delete from r_wms_location where ROW in ('06','08','10','12') and BAY in ('025','026','047','048','071','072') and LEVEL in ('06','07');
delete from r_wms_location where ROW = '14' and BAY in ('004','005','025','026','047','048','071','072') and LEVEL in ('04','05');
delete from r_wms_location where ROW in ('13','14') and BAY in ('001','002') and LEVEL < '06';
delete from r_wms_location where ROW = '14' and BAY in ('006','027','049','073') and LEVEL < '06';
delete from r_wms_location where ROW = '14' and BAY = '003' and LEVEL < '04';
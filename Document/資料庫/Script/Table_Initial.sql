------------Sno_Max -------------
INSERT INTO Sno_Max (Sno_Type,Month_Flag,Init_Sno,Max_Sno,Sno_Len) 
VALUES 
('CMDSNO', 'Y', 1, 19997, 5),
('CMDSUO', 'N', 20001, 29997, 5);

INSERT INTO UnitModeDef (StockerID, In_enable) VALUES
('1', 'Y'),
('2', 'Y'),
('3', 'Y'),
('4', 'Y');

insert into Teach_Loc (DeviceID,Loc) VALUES
('1','0200303'),
('2','0200303'),
('3','0200303');

--STK 1
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'A1_01', 2, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'A1_02', 2, 2, 2, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'A1_04', 2, 3, 4, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'A1_05', 2, 4, 5, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'LeftFork', 4, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'RightFork', 4, 2, 2, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'Shelf', 0, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('1', 'Teach', 0, 2, 2, ' ');


--STK 2
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'A1_07', 2, 1, 7, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'A1_08', 2, 2, 8, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'A1_10', 2, 3, 10, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'A1_11', 2, 4, 11, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'LeftFork', 4, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'RightFork', 4, 2, 2, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'Shelf', 0, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('2', 'Teach', 0, 2, 2, ' ');


--STK 3
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'A1_13', 2, 1, 13, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'A1_14', 2, 2, 14, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'A1_16', 2, 3, 16, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'A1_17', 2, 4, 17, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'LeftFork', 4, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'RightFork', 4, 2, 2, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'Shelf', 0, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('3', 'Teach', 0, 2, 2, ' ');


--STK 4
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'A1_19', 2, 1, 19, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'A1_20', 2, 2, 20, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'A1_22', 2, 3, 22, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'A1_23', 2, 4, 23, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'LeftFork', 4, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'RightFork', 4, 2, 2, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'Shelf', 0, 1, 1, ' ');
insert into PortDef (DeviceID, HostPortID, PortType, PortTypeIndex, PLCPortID, TrnDT) values('4', 'Teach', 0, 2, 2, ' ');
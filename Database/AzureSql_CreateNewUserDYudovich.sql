CREATE LOGIN [dyudovich] WITH Password = 'X-vZUbw5CCaMPEPc@FGBloJojlIxi65KjY#RGC85rcrMI-dH2Dd7fsTQqEQC3lr4'
CREATE USER [dyudovich] FROM LOGIN [dyudovich];
ALTER ROLE [db_datareader] ADD MEMBER [dyudovich];
ALTER ROLE [db_datawriter] ADD MEMBER [dyudovich];


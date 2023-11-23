-- Database creation and filling script
-- Execute in strict sequence
-- Developed by A.Tarasenko 21.11.2023
-- Before running the script, make sure that the DEPARTMENTS and EMP_DATA tables do not exist in the DB
-- for dropiing use:  
-- EXECUTE IMMEDIATE 'DROP TABLE DEPARTMENTS'; 
-- EXECUTE IMMEDIATE 'DROP TABLE EMP_DATA';
-- EXECUTE IMMEDIATE 'DROP TABLE ERR_MSG';

--==========================================
-- Creating the service tables
-- of the error log 
  CREATE TABLE "ERR_MSG" 
   ("DAT" DATE DEFAULT sysdate NOT NULL ENABLE, 
	"ERR" VARCHAR2(200), -- error code
	"MSG" VARCHAR2(200)  -- error source and msg
   ) ;    
  /
--=====================
  CREATE TABLE "DEPARTMENTS" 
   ("DEPT_ID" NUMBER GENERATED BY DEFAULT ON NULL AS IDENTITY MINVALUE 1 MAXVALUE 99999 INCREMENT BY 1 START WITH 1 CACHE 20 NOORDER  NOCYCLE  NOKEEP  NOSCALE  NOT NULL ENABLE, 
    "DEPT_NAME" VARCHAR2(50 CHAR) NOT NULL ENABLE, 
    "LOCATION" VARCHAR2(50 CHAR) NOT NULL ENABLE, 
     CONSTRAINT "DEPARTMENTS_PK" PRIMARY KEY ("DEPT_ID")
  USING INDEX  ENABLE
   ) ;
   COMMENT ON COLUMN "DEPARTMENTS"."DEPT_ID" IS 'The unique identifier for the department';
   COMMENT ON COLUMN "DEPARTMENTS"."DEPT_NAME" IS 'The name of the department';
   COMMENT ON COLUMN "DEPARTMENTS"."LOCATION" IS ' The physical location of the department';
   COMMENT ON TABLE "DEPARTMENTS"  IS 'Departments information';
/   
-- Filling in the information for DEPARTMENTS 
begin 
    insert into DEPARTMENTS (dept_id,dept_name,location) values (1,'Management','London');
    insert into DEPARTMENTS (dept_id,dept_name,location) values (2,'Engineering','Cardiff');
    insert into DEPARTMENTS (dept_id,dept_name,location) values (3,'Research & Development','Edinburgh');
    insert into DEPARTMENTS (dept_id,dept_name,location) values (4,'Sales','Belfast');
end;      
/   
   
  CREATE TABLE "EMP_DATA" 
   ("EMP_ID" NUMBER GENERATED BY DEFAULT ON NULL AS IDENTITY MINVALUE 1 MAXVALUE 9999999999 INCREMENT BY 1 START WITH 90011 NOORDER  NOCYCLE  NOKEEP  NOSCALE  NOT NULL ENABLE, 
    "ENAME" VARCHAR2(50 CHAR) NOT NULL ENABLE, 
    "JOB" VARCHAR2(50 CHAR) NOT NULL ENABLE, 
	"MGR" NUMBER(10,0), 
	"HIREDATE" DATE NOT NULL ENABLE, 
	"SAL" NUMBER(10,0) NOT NULL ENABLE, 
	"DEPT_ID" NUMBER(5,0) NOT NULL ENABLE, 
	 CONSTRAINT "EMP_DATA_PK" PRIMARY KEY ("EMP_ID")
  USING INDEX  ENABLE
   ) ;

   COMMENT ON COLUMN "EMP_DATA"."EMP_ID" IS 'The unique identifier for the employee';
   COMMENT ON COLUMN "EMP_DATA"."ENAME" IS 'The name of the employee';
   COMMENT ON COLUMN "EMP_DATA"."JOB" IS 'The job role undertaken by the employee. Some employees may
                                            undertaken the same job role';
   COMMENT ON COLUMN "EMP_DATA"."MGR" IS 'Line manager of the employee';
   COMMENT ON COLUMN "EMP_DATA"."HIREDATE" IS 'The date the employee was hired';
   COMMENT ON COLUMN "EMP_DATA"."SAL" IS 'Current salary of the employee';
   COMMENT ON COLUMN "EMP_DATA"."DEPT_ID" IS 'Each employee must belong to a department';
   COMMENT ON TABLE "EMP_DATA"  IS 'Employees File';
   ALTER TABLE "EMP_DATA" ADD FOREIGN KEY ("DEPT_ID")
	  REFERENCES "DEPARTMENTS" ("DEPT_ID") ENABLE;
/

-- Creating service package
create or replace package "PKG_EMP_SRV" as
    --==============================================================================
    -- add EMP_DATA record
    --==============================================================================
    procedure p_add_emp (       
        p_emp_id     in     number default null,    -- employee ID
        p_ename      in     varchar2,               -- employee name        
        p_job        in     varchar2,               -- employee job name
        p_mgr        in     number default null,    -- manager
        p_hiredate   in     date default sysdate,   -- when hired
        p_sal        in     number,                 -- salary
        p_dept_id     in     number                 -- dapartment ID 
        );
    function f_add_emp (
        p_emp_id     in     number default null,    -- employee ID
        p_ename      in     varchar2,               -- employee name        
        p_job        in     varchar2,               -- employee job name
        p_mgr        in     number default null,    -- manager
        p_hiredate   in     date default sysdate,   -- when hired
        p_sal        in     number,                 -- salary
        p_dept_id     in     number                 -- dapartment ID
        ) 
            -- returning ID of new record
            -- if return -1 then error. Check details in ERR_LOG table
            return number;
    --==============================================================================
    -- add  record into error log
    --==============================================================================
    procedure p_add_err_log(
        p_date in date default sysdate,
        p_err in  varchar2,
        p_msg in varchar2);
    --==============================================================================
    -- procedure of salary chaging
    -- p_percent = value in percents, can be negative
    --==============================================================================
    procedure p_change_sal(
        p_emp_id in number,
        p_percent in number,
        p_date_of_change in date default sysdate);
    --==============================================================================
    -- procedure EMP transfering employeer to another department    
    --==============================================================================
    procedure p_emp_transfer(
        p_emp_id in number,
        p_new_dept in number,
        p_date_of_change in date default sysdate);
    --==============================================================================
    -- PROCEDURE/function of salary - return current salary by emp
    --==============================================================================
    procedure p_return_sal(
        p_emp_id in number);
    function f_return_sal(
        p_emp_id in number) return number;
    --==============================================================================
    -- procedure report dept/dapts if p_dept_id is null then return whole report
    --==============================================================================
    procedure p_dept_rep(
        p_dept_id in number default null);            
    --==============================================================================
    -- procedure report salary for dept 
    --==============================================================================
    procedure p_sal_rep(
        p_dept_id in number default null);            
    --==============================================================================
    -- procedure whole unit test
    --==============================================================================
    procedure p_utest;
end "PKG_EMP_SRV";
/
create or replace package body "PKG_EMP_SRV" as
    --==============================================================================
    -- procedure/function for add EMP_DATA record
    -- declare v_ret number; begin v_ret := PKG_EMP_SRV.f_add_emp(null.'Alex', 'Employee',,,148000,1); end;
    --==============================================================================
    procedure p_add_emp (       
        p_emp_id     in     number default null,    -- employee ID
        p_ename      in     varchar2,               -- employee name        
        p_job        in     varchar2,               -- employee job name
        p_mgr        in     number default null,    -- manager
        p_hiredate   in     date default sysdate,   -- when hired
        p_sal        in     number,                 -- salary
        p_dept_id     in     number                 -- dapartment ID
        )  is
        v_ret number;
        begin
            -- readressing to the function
            v_ret := f_add_emp(p_emp_id,p_ename,p_job,p_mgr,p_hiredate,p_sal,p_dept_id);
        end;    
    function f_add_emp (       
        p_emp_id     in     number default null,    -- employee ID
        p_ename      in     varchar2,               -- employee name        
        p_job        in     varchar2,               -- employee job name
        p_mgr        in     number default null,    -- manager
        p_hiredate   in     date default sysdate,   -- when hired
        p_sal        in     number,                 -- salary
        p_dept_id     in     number                 -- dapartment ID
        ) return number is
        v_new_id NUMBER;
        begin
            -- inserting new employee
            INSERT into EMP_DATA (EMP_ID, ENAME, JOB, MGR, HIREDATE, SAL, DEPT_ID)            
                values (p_emp_id, p_ename, p_job, p_mgr, p_hiredate, p_sal, p_dept_id) 
                    RETURNING EMP_ID INTO v_new_id;
            -- messsage for SQL
            dbms_output.put_line('EMP record added. EMP_ID = ' || v_new_id);
            -- messsage for WEB
            htp.p('EMP record added. EMP_ID = ' || v_new_id ||'<br>');
            return v_new_id;

            exception
                when others then 
                -- error handling
                -- $$PLSQL_UNIT - return object name
                p_add_err_log(sysdate,SQLCODE,SUBSTR($$PLSQL_UNIT  || '.f_add_emp  @ ' || SQLERRM, 1, 200) );
                dbms_output.put_line('EMP record did not add. EMP_ID = ' || SQLERRM);
                htp.p('EMP record did not add. EMP_ID = ' || SQLERRM ||'<br>');

                return -1; 
        end;
    --==============================================================================
    -- add  record into error log
    --==============================================================================        
    procedure p_add_err_log(
        p_date in date default sysdate,
        p_err in  varchar2,
        p_msg in varchar2) is
        begin
            insert into err_msg (DAT,ERR,MSG) values (p_date, p_err, p_msg);
            exception
            -- error handling
                when others then null;
        end;
    --==============================================================================
    -- procedure of salary chaging
    -- p_percent = value in percents, can be negative
    --==============================================================================
    procedure p_change_sal(
        p_emp_id in number,
        p_percent in number,                            -- percentage size. can be a negative
        p_date_of_change in date default sysdate) is
        cursor c_emp is select * from emp_data where emp_id = p_emp_id;
        t_emp c_emp%ROWTYPE;
        begin
            open c_emp;
            fetch c_emp into t_emp;
            if c_emp%notfound then
                -- something wrong. employee not found
                close c_emp;
                dbms_output.put_line('EMP not found. EMP_ID = ' || p_emp_id);
                htp.p('EMP not found. EMP_ID = ' || p_emp_id||'<br>');
                return;
            end if;
            close c_emp;
            update emp_data set
                -- calculating a new salary
                sal = trunc(t_emp.sal + t_emp.sal/100 * p_percent)
                where emp_id = p_emp_id; 
                       insert into SAL_LOG (EMP_ID,DATE_LOG,SAL)
                        values (p_emp_id,sysdate,t_emp.sal);
                dbms_output.put_line('EMP salary updated on ' || p_percent || '%. EMP_ID = ' || p_emp_id || '  ' || 'New salary = ' || trunc(t_emp.sal + t_emp.sal/100 * p_percent) );
                htp.p('EMP salary updated on ' || p_percent || '%. EMP_ID = ' || p_emp_id || '  ' || 'New salary = ' || trunc(t_emp.sal + t_emp.sal/100 * p_percent) ||'<br>');

           exception
                when others then 
                -- error handling
                p_add_err_log(sysdate,SQLCODE,SUBSTR($$PLSQL_UNIT  || '.p_change_sal  @ ' || SQLERRM, 1, 200) );                
        end;
    --==============================================================================
    -- procedure EMP transfering     
    --==============================================================================
    procedure p_emp_transfer(
        p_emp_id in number,
        p_new_dept in number,    -- new department ID. 
        p_date_of_change in date default sysdate) is 
        begin
           update emp_data set 
                DEPT_ID =  p_new_dept
                where emp_id = p_emp_id;
            dbms_output.put_line('Employee transfered. EMP_ID = ' || p_emp_id || ' New DEPT_ID = ' || p_new_dept);
            htp.p('Employee transfered. EMP_ID = ' || p_emp_id || ' New DEPT_ID = ' || p_new_dept||'<br>');
           exception
                when others then 
                -- error handling
                p_add_err_log(sysdate,SQLCODE,SUBSTR($$PLSQL_UNIT  || '.p_emp_transfer  @ ' || SQLERRM, 1, 200) );
                dbms_output.put_line('Error during operation. Probably wrong DEPT_ID parameter');
                htp.p('Error during operation. Probably wrong DEPT_ID parameter'||'<br>');

        end;          
    --==============================================================================
    -- PROCEDURE/function of salary 
    --==============================================================================
    procedure p_return_sal(
        p_emp_id in number) is
        v_ret  number;
       begin
            V_ret := f_return_sal(p_emp_id);
       end;
    function f_return_sal(
        p_emp_id in number) return number is
                cursor c_sal is select SAL from emp_data where emp_id = p_emp_id;
        v_sal number;        
        begin
            open c_sal;
            fetch c_sal into v_sal;
            close c_sal;
            dbms_output.put_line('Employee EMP_ID = ' || p_emp_id || ' Salary = ' || v_sal);
            htp.p('Employee EMP_ID = ' || p_emp_id || ' Salary = ' || v_sal||'<br>');
            return v_sal;
           exception
           -- error handling
                when others then 
                p_add_err_log(sysdate,SQLCODE,SUBSTR($$PLSQL_UNIT  || '.f_return_sal  @ ' || SQLERRM, 1, 200) );
        end; 
    --==============================================================================
    -- procedure report dept
    --==============================================================================
    procedure p_dept_rep(
        p_dept_id in number default null) is
        cursor c_emp is 
            select emp.* , dept.dept_name from departments dept
                join emp_data emp on (dept.dept_id = emp.dept_id)
                    where dept.dept_id = nvl(p_dept_id, dept.dept_id)
                        order by dept.dept_id, emp.emp_id;
        begin
           dbms_output.put_line(RPAD('DEPT_ID',20) || RPAD('DEPT_NAME',20) || RPAD('EMP_ID',20) || RPAD('ENAME',20) || RPAD('HIRE DATE',20)   );
           htp.htmlopen();
           htp.bodyopen();
           htp.p('<table border = "1"><tr><td><b>DEPT_ID</b></td><td><b>DEPT_NAME</b></td><td><b>EMP_ID</b></td></b><td><b>ENAME</b></td><td><b>HIRE DATE</b></td></tr>');
           for i in c_emp
           loop
                dbms_output.put_line(RPAD(i.DEPT_ID,20) || RPAD(i.DEPT_NAME,20) || RPAD(i.EMP_ID,20) || RPAD(i.ENAME,20) || RPAD(to_char(i.HIREDATE,'dd.mm.yyyy'),20));
                htp.tablerowopen();
                    htp.tabledata(i.DEPT_ID);
                    htp.tabledata(i.DEPT_NAME);
                    htp.tabledata(i.EMP_ID);
                    htp.tabledata(i.ENAME);
                    htp.tabledata(to_char(i.HIREDATE,'dd.mm.yyyy'));
                htp.tablerowclose();
           end loop;
           htp.tableclose();
           htp.p('<br>');
           htp.bodyclose();
           htp.htmlclose();
           exception
                when others then 
                p_add_err_log(sysdate,SQLCODE,SUBSTR($$PLSQL_UNIT  || '.p_dept_rep  @ ' || SQLERRM, 1, 200) );            
        end; 
    --==============================================================================
    -- procedure report salary for dept
    --==============================================================================
    procedure p_sal_rep(
        p_dept_id in number default null) is
        cursor c_emp is 
            select dept.dept_ID, dept.dept_name, SUM(SAL) SAL, trunc(AVG(SAL)) SAVG from departments dept
                join emp_data emp on (dept.dept_id = emp.dept_id)
                    where dept.dept_id = nvl(p_dept_id, dept.dept_id)
                        GROUP BY dept.dept_ID, dept.dept_name
                        order by dept.dept_ID;        
        begin
           dbms_output.put_line(RPAD('DEPT_ID',20) || RPAD('DEPT_NAME',20) || RPAD('Total salary',20)|| RPAD('Avarage',20));
           htp.htmlopen();
           htp.bodyopen();
           htp.p('<table border = "1"><tr><td><b>DEPT_ID</b></td><td><b>DEPT_NAME</b></td><td><b>Total salary</b></td><td><b>Avarage salary</b></td></tr>');
           for i in c_emp
           loop
                dbms_output.put_line(RPAD(i.DEPT_ID,20) || RPAD(i.DEPT_NAME,20) || RPAD(i.SAL,20)|| RPAD(i.SAVG,20));
                htp.tablerowopen();
                    htp.tabledata(i.DEPT_ID);
                    htp.tabledata(i.DEPT_NAME);
                    htp.tabledata(i.SAL);
                    htp.tabledata(i.SAVG);
                htp.tablerowclose();
           end loop;
           htp.tableclose();
           htp.p('<br>');
           htp.bodyclose();
           htp.htmlclose();
           exception
                when others then 
                -- error handling
                p_add_err_log(sysdate,SQLCODE,SUBSTR($$PLSQL_UNIT  || '.p_sal_rep  @ ' || SQLERRM, 1, 200) );            
        end;  
--==============================================================================
-- procedure whole unit test
--==============================================================================        
        procedure p_utest 
            is 
            v_EMP number := 90021;
            begin
                dbms_output.put_line('Add new employeer');
                v_EMP := PKG_EMP_SRV.f_add_emp(null,'TEST EMP', 'TEST EMP',null,sysdate,148000,1); 
                dbms_output.put_line('Displaying the report with this employeer');
                PKG_EMP_SRV.p_dept_rep(p_dept_id => 1);
                dbms_output.put_line('Checking current salary');
                PKG_EMP_SRV.p_return_sal(p_emp_id => v_EMP);  
                dbms_output.put_line('Salary doubling');
                PKG_EMP_SRV.p_change_sal(p_emp_id => v_EMP,p_percent => 100);
                dbms_output.put_line('Checking new salary');
                PKG_EMP_SRV.p_return_sal(p_emp_id => v_EMP);  
                dbms_output.put_line('Transfering to DEPT = 2 ');
                PKG_EMP_SRV.p_emp_transfer(p_emp_id => v_EMP,p_new_dept => 2);
                dbms_output.put_line('Displaying the report with this employeer in DEPT = 2');
                PKG_EMP_SRV.p_dept_rep(p_dept_id => 2);
                dbms_output.put_line('Checking total salary information');
                htp.p('Checking total salary information');
                PKG_EMP_SRV.p_sal_rep(p_dept_id => null);
                dbms_output.put_line('Test employeer EMP_ID = ' || v_EMP ||' deleted');                
                htp.p('Test employeer EMP_ID = ' || v_EMP ||' deleted');                
                delete emp_data where emp_id = v_EMP;
                commit;
            end;


end "PKG_EMP_SRV";
/
-- Filling in the information for EMP_DATA
declare v_ret number;
    begin 
        v_ret := pkg_emp_srv.f_add_emp(90001,'John Smith','CEO',null,to_date('1.1.1995','dd.mm.yyyy'),100000,1);
        v_ret := pkg_emp_srv.f_add_emp(90002,'Jimmy Willis','Manager',90001,to_date('23.9.2003','dd.mm.yyyy'),52500,4);
        v_ret := pkg_emp_srv.f_add_emp(90003,'Roxy Jones','Salesperson',90002,to_date('11.2.2017','dd.mm.yyyy'),35000,4);
        v_ret := pkg_emp_srv.f_add_emp(90004,'Selwyn Field','Salesperson',90003,to_date('20.5.2015','dd.mm.yyyy'),32000,4);
        v_ret := pkg_emp_srv.f_add_emp(90005,'David Hallett','Engineer',90006,to_date('17.4.2018','dd.mm.yyyy'),40000,2);
        v_ret := pkg_emp_srv.f_add_emp(90006,'Sarah Phelps','Manager',90001,to_date('21.3.2015','dd.mm.yyyy'),45000,2);
        v_ret := pkg_emp_srv.f_add_emp(90007,'Louise Harper','Engineer',90006,to_date('1.1.2013','dd.mm.yyyy'),47000,2);
        v_ret := pkg_emp_srv.f_add_emp(90008,'Tina Hart','Engineer',90009,to_date('28.7.2014','dd.mm.yyyy'),45000,3);
        v_ret := pkg_emp_srv.f_add_emp(90009,'Gus Jones','Manager',90001,to_date('15.5.2018','dd.mm.yyyy'),50000,3);
        v_ret := pkg_emp_srv.f_add_emp(90010,'Mildred Hall','Secretary',90001,to_date('12.10.1996','dd.mm.yyyy'),35000,1);
    end;
    
/
-- Adding last FK. Fixing data integrity
  ALTER TABLE "EMP_DATA" ADD FOREIGN KEY ("MGR")
	  REFERENCES "EMP_DATA" ("EMP_ID") ENABLE;
/
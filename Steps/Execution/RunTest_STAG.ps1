try{
	#basic path variable
	$Env = "STAG"
	$BasePath = "D:\Testautomation\ACC" 
	$SMTP_SERVER = "smtpgw.innogy.com"
	$FileMsgToSlack = "D:\Testautomation\ACC\Execution\MsgToSlack.txt"
	# used for local debug
	#$BasePath = "Y:\Source\Repos\E2ETestAutomation\TestAutomation\bin\Debug"
	
	$ArchivePath = (Get-Item $BasePath).Parent.Fullname
	$Date = Get-Date -format "yyyyMMddHHmm"
	$TestResultsFile = "$BasePath\out_$Env\TestResults\TestReport_$Env_$Date.html"
	$TestReportFileName = "TestReport_$Env_$Date.html" 
    $profile = "Profiles\$Env.srprofile"
   
	$Source = "$BasePath\out_$Env\TestResults"
	$ResultZip = "$BasePath\TestResults_$Env_"+$(get-date -f yyyyMMddHHmm)+".zip"

	$allureBase = "D:\allure"
	$allureFolder= "$allureBase\allure-reports_$Env"
    $allureResults= "$Source\allure-results"
	$onlineResults= "$allureBase\results_$Env"
    
    $onlineResultsURL = "https://ta-report.rwe.com"

	$mailinglist =  "aik.tan.extern@innogy.com", "frank.bieniek.extern@innogy.com", "jozef.dzivy.extern@innogy.com", "jan.ryjacek.extern@innogy.com", "pascal.schaefer.extern@innogy.com"
	$mailinglistError = "jozef.dzivy.extern@innogy.com", "aik.tan.extern@innogy.com", "pascal.schaefer.extern@innogy.com"
	
	#register licence	 
	&$BasePath\SpecFlowPlusRunner\SpecRun.exe register KI7LYDZsATIICqe1vhtZ0QBVnGdZFYgLASLu/6Dvo2W2sSqZ+Kr1JEABAWTzAAAAAA== " innogy SE "
	
    #Run tests	
		write-host "STARTING WAKE UP"
		Start Chrome "https://cd-ele-acc.innogy.com/privatkunden/meine-ele/services"
		Start-Sleep 30.0
		Start Chrome "https://cd-ele-acc.innogy.com/privatkunden/meine-ele/services/daten"
		Start-Sleep 30.0
		Start Chrome "https://cd-ele-acc.innogy.com/privatkunden/meine-ele/services/abschlaege"
		Start-Sleep 30.0
		Start Chrome "https://cd-ele-acc.innogy.com/privatkunden/meine-ele/services/vertraege"
		Start-Sleep 120.0
		#Kill Chrome processes
		$chrome = Get-Process chrome -ErrorAction SilentlyContinue
		$chrome_driver = Get-Process chromedriver -ErrorAction SilentlyContinue
    
		if($chrome){
			write-host "chrome.exe still running"
			."taskkill.exe" /F /T /IM chrome.exe
			write-host "Killed all chrome.exe instances"
		}
		if($chrome_driver){
			write-host "chromedriver.exe still running"
			."taskkill.exe" /F /T /IM chromedriver.exe
			write-host "Killed all chromedriver.exe instances"
		}


    write-host "STARTING EXECUTION"
	$report = &$BasePath\SpecFlowPlusRunner\SpecRun.exe run $profile /basefolder:$BasePath /outputfolder:$BasePath\out_$Env\TestResults /report:$TestReportFileName /log:log_$Date.txt

		#Kill Chrome processes
		$chrome = Get-Process chrome -ErrorAction SilentlyContinue
		$chrome_driver = Get-Process chromedriver -ErrorAction SilentlyContinue
		if($chrome){
			write-host "chrome.exe still running"
			."taskkill.exe" /F /T /IM chrome.exe
			write-host "Killed all chrome.exe instances"
		}
		if($chrome_driver){
			write-host "chromedriver.exe still running"
			."taskkill.exe" /F /T /IM chromedriver.exe
			write-host "Killed all chromedriver.exe instances"
		}


	write-host "Report: " $report
    $report = ""+$report
    $found = $report -match '\sResult:.*Report\sfile'   
    if (!$found) {
       write-host "not found match, no extract" 
    }else {
        $report =  $matches[0]     
        $index = $report.LastIndexOf('Report')      
        $report = $report.Substring("0",$index-1)
    }
	
	write-host "exitcode: " $LASTEXITCODE

    write-host "report: " $report

	$result =""

	switch ( $LASTEXITCODE )
		{
			0 { $result = 'Unknown'    }
			110 { $result = 'Succeeded'    }
			120 { $result = 'Ignored'   }
			210 { $result = 'Pending' }
			310 { $result = 'NothingToRun' }
			320 { $result = 'Skipped'  }
			410 { $result = 'Inconclusive'    }
			420 { $result = 'CleanupFailed'  }
			430 { $result = 'RandomlyFailed'  }
			440 { $result = 'Failed'  }
			450 { $result = 'InitializationFailed'  }
			510 { $result = 'FrameworkError'  }
			520 { $result = 'ConfigurationError'  }
		}

    #copy old history of allure report
	Copy-Item "$allureFolder\history" "$allureResults\history" -force -recurse
	
    #copy allure customized defect categorysation to allure results folder
    Copy-Item "$BasePath\Setup\allure\categories.json" "$allureResults"
    #generate allure report, which is installed in machine
	&$allureBase\bin\allure.bat generate --clean $allureResults --output $allureFolder

	#used for local debug:
    #&allure generate --clean $allureResults --output $allureFolder

    #clear online result folder LAST
	Remove-Item "$onlineResults\last\*" -force -recurse

	#copy results to online results folder LAST
	Copy-Item "$Source\*" "$onlineResults\last" -force -recurse

	#rename result file to index.html
	Rename-Item -Path "$onlineResults\last\$TestReportFileName" -NewName "index.html"
	
	write-host "ARCHEIVING"
	#creating of archive folder
    $ArchiveFolderTime =  "\$((Get-Date -f yyyyMMdd))" + "\$(get-date -f HHmmss)"
	$ArchiveFolder = "$ArchivePath\ArchiveE2E\" +$Env+ $ArchiveFolderTime
	If (Test-Path $ArchiveFolder){
	  write-host "Folder already exists!"
	}Else{
	  New-item $ArchiveFolder -itemtype directory
	}

	#compressing of screenshots folder
	write-host "COMPRESSION"
	 If(Test-path $ResultZip){
		write-host "Folder already exists!"
	 }Else{
		Add-Type -assembly "system.io.compression.filesystem"
		[io.compression.zipfile]::CreateFromDirectory($Source, $ResultZip) 
	}

	#sending email with attachments: log from powershell script, log from NUnit Console Runner, test result in XML and HTML, screenshots folder
	try{
		write-host "SENDING EMAIL:"
        $ArchiveFolderTime = $ArchiveFolderTime.replace("\","/")
		Send-MailMessage -SmtpServer $SMTP_SERVER -Port 25 -From "noreply@innogy.com" -To $mailinglist -Body "Results of automated E2E tests (environment: $Env, result $result)  `r`n `r`n ALLURE REPORT: $onlineResultsURL/$Env `r`n `r`n ONLINE REPORT OF LAST RESULTS: $onlineResultsURL/$Env/results/last `r`n RESULT IN ARCHIVE: $onlineResultsURL/$Env/results/history$ArchiveFolderTime `r`n HISTORY OF ALL RESULTS: $onlineResultsURL/$Env/results/history `r`n `r`n REPORT: `r`n $report" -Subject "Results of automated E2E tests (environment: $Env, result $result)" -Attachments $TestResultsFile
		#Send-MailMessage -SmtpServer smtpgw.rwe.com -Port 25 -From "noreply@innogy.com" -To $mailinglist -Body "Results of automated E2E tests (environment: $Env, result $result)  `r`n `r`n ALLURE REPORT: $onlineResultsURL/$Env `r`n `r`n ONLINE REPORT OF LAST RESULTS: $onlineResultsURL/$Env/results/last `r`n RESULT IN ARCHIVE: $onlineResultsURL/$Env/results/history$ArchiveFolderTime `r`n HISTORY OF ALL RESULTS: $onlineResultsURL/$Env/results/history `r`n `r`n REPORT: `r`n $report `r`n `r`n MONITORING:  " -Subject "Results of automated E2E tests (environment: $Env, result $result)" -Attachments $TestResultsFile
        #Send-MailMessage -SmtpServer smtpgw.rwe.com -Port 25 -From "noreply@innogy.com" -To $mailinglist -Body "Results of automated E2E tests (environment: $Env, result $result) `r`n `r`n ONLINE REPORT OF LAST RESULTS: $onlineResultsURL/$Env/results/last `r`n  RESULT IN ARCHIVE: $onlineResultsURL/$Env/results/history$ArchiveFolderTime `r`n HISTORY OF ALL RESULTS: $onlineResultsURL/$Env/results/history `r`n `r`n REPORT: `r`n $report" -Subject "Results of automated E2E tests (environment: $Env, result $result)" -Attachments $TestResultsFile,$ResultZip
				
        #Send-MailMessage -SmtpServer smtpgw.rwe.com -Port 25 -From "noreply@innogy.com" -To "m.dzuris.extern@innogy.com" -Body "Results of automated E2E tests (environment: ACC, result $result) `r`n `r`n REPORT: `r`n $report" -Subject "Results of automated E2E tests (environment: ACC, result $result)" -Attachments $TestResultsFile, $ResultZip
	
	}
	catch {
		write-host "ERROR AFTER SENDING EMAIL:"
		write-error $_.Exception.Message
		echo $_.Exception.Message
		Send-MailMessage -SmtpServer  $SMTP_SERVER -Port 25 -From "noreply@innogy.com" -To $mailinglistError  -Subject "ERROR: Results of automated E2E tests (environment: ACC)" -Body $_.Exception.Message
	
	}
	finally
	{
		write-host "MOVEING AND REMOVING"
        Move-Item $TestResultsFile $ArchiveFolder -force
		Move-Item $ResultZip $ArchiveFolder -force		
        Remove-Item $Source\* -force -recurse
		
		#Remove-Item $ResultZip -force
		$chrome = Get-Process chrome -ErrorAction SilentlyContinue
        $chrome_driver = Get-Process chromedriver -ErrorAction SilentlyContinue
    
        if($chrome){
           write-host "chrome.exe still running"
           ."taskkill.exe" /F /T /IM chrome.exe
           write-host "Killed all chrome.exe instances"
       }
       if($chrome_driver){
          write-host "chromedriver.exe still running"
          ."taskkill.exe" /F /T /IM chromedriver.exe
          write-host "Killed all chromedriver.exe instances"
       }
	}
}
catch{
	write-host "ERROR"
	$_ | Out-File $BasePath\errors.txt -Append
	write-error $_
	$ErrorFile = "$BasePath\errors.txt"
	Send-MailMessage -SmtpServer  $SMTP_SERVER -Port 25 -From "noreply@innogy.com" -To $mailinglistError -Body "Results of automated E2E tests (environment: $Env), see attachment for details" -Subject "Results of automated E2E tests (environment: $Env)" -Attachments $ErrorFile
	Remove-Item $ErrorFile -force
	write-host "END WITH ERROR"
	Break
}
finally{
	$chrome = Get-Process chrome -ErrorAction SilentlyContinue
    $chrome_driver = Get-Process chromedriver -ErrorAction SilentlyContinue
    
    if($chrome){
        write-host "chrome.exe still running"
        ."taskkill.exe" /F /T /IM chrome.exe
        write-host "Killed all chrome.exe instances"
    }
    if($chrome_driver){
        write-host "chromedriver.exe still running"
        ."taskkill.exe" /F /T /IM chromedriver.exe
        write-host "Killed all chromedriver.exe instances"
    }
}

	#Send notification to Slack
	$SendMsgToSlack = "Test Automation finished on environment $Env , $report"
	write-host "Preparing to Slack information about results:" $SendMsgToSlack
	if(Test-path $FileMsgToSlack){
		Remove-Item $FileMsgToSlack -force -recurse
		write-host "FileMsgToSlack moved! " + $FileMsgToSlack
	 }
	 New-Item -ItemType "file" -Path $FileMsgToSlack -Value $SendMsgToSlack
	 Start-ScheduledTask -TaskName "AutomatedTestsSlack"

write-host "END"
namespace Ensek.Net.MeterReading.Api.Processing;

public class MeterReadingProcessor : IMeterReadingProcessor
{
    private readonly IMeterReadingsRepository _meterReadingsRepository;

    public MeterReadingProcessor(IMeterReadingsRepository meterReadingsRepository)
    {
        Guard.Against.Null(meterReadingsRepository, nameof(meterReadingsRepository));
        _meterReadingsRepository = meterReadingsRepository;
    }

    public MeterReadingFileUploadResponse Process(int auditId, List<ImportFileAudit> importFileAudits)
    {
        Guard.Against.NegativeOrZero(auditId, nameof(auditId));
        Guard.Against.Null(importFileAudits, nameof(importFileAudits));

        var uploadResponse = new MeterReadingFileUploadResponse
        {
            ImportFailureReasons = new List<ImportFileAudit>()
        };

        foreach (var importFileAudit in importFileAudits)
        {
            var validationProblem = string.Empty;
            var recordImport = new ImportFileAudit(importFileAudit.AccountId, importFileAudit.MeterReadingDateTime, importFileAudit.MeterReadingValue);
            var accountId = ValidateAccountId(importFileAudit, ref validationProblem);
            var meterReadingDateTime = ValidateMeterReadingDateTime(importFileAudit, ref validationProblem);
            var meterReadingValue = ValidateMeterReadingValue(importFileAudit, ref validationProblem);
            if (validationProblem == string.Empty)
            {
                try
                {
                    _meterReadingsRepository.CreateNewMeterReadingRecord(accountId, meterReadingDateTime, meterReadingValue, auditId, false);
                    uploadResponse.NumberOfRecordsSuccessfullyImported++;
                }
                catch (MeterReadingRecordNotCreatedException ex)
                {
                    validationProblem += ex.Message + ",";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    validationProblem += ex.Message + ",";
                }
                catch (ArgumentException ex)
                {
                    validationProblem += ex.Message + ",";
                }
            }
            if (validationProblem != string.Empty)
            {
                uploadResponse.NumberOfRecordsFailedToImport++;
                recordImport.FailureReason = validationProblem.TrimEnd(',');
                uploadResponse.ImportFailureReasons.Add(recordImport);
            }
        }

        return uploadResponse;
    }

    private int ValidateMeterReadingValue(ImportFileAudit importFileAudit, ref string validationProblem)
    {
        int meterReadingValue = -1;
        if (string.IsNullOrWhiteSpace(importFileAudit.MeterReadingValue))
        {
            validationProblem += "meter reading value is not provided,";
        }
        else
        {
            var meterReadingValueParsed = int.TryParse(importFileAudit.MeterReadingValue, out meterReadingValue);
            if (!meterReadingValueParsed)
            {
                validationProblem += "meter reading value is not valid,";
            }
            else if (meterReadingValue < 0)
            {
                validationProblem += "meter reading value cannot be a negative number,";
            }
        }
        return meterReadingValue;
    }

    private DateTime ValidateMeterReadingDateTime(ImportFileAudit importFileAudit, ref string validationProblem)
    {
        var meterReadingDateTime = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(importFileAudit.MeterReadingDateTime))
        {
            validationProblem += "meter reading date time is not provided,";
        }
        else
        {
            var meterReadingDateTimeParsed = DateTime.TryParse(importFileAudit.MeterReadingDateTime, out meterReadingDateTime);
            if (!meterReadingDateTimeParsed)
            {
                validationProblem += "meter reading date time is not valid,";
            }
            else if (meterReadingDateTime == DateTime.MinValue)
            {
                validationProblem += "meter reading date time cannot have a minimum value,";
            }
        }
        return meterReadingDateTime;
    }

    private int ValidateAccountId(ImportFileAudit importFileAudit, ref string validationProblem)
    {
        var accountId = 0;
        if (string.IsNullOrWhiteSpace(importFileAudit.AccountId))
        {
            validationProblem += "account id is not provided,";
        }
        else
        {
            var accountIdParsed = int.TryParse(importFileAudit.AccountId, out accountId);
            if (!accountIdParsed)
            {
                validationProblem += "account id is not valid,";
            }
            else if (accountId <= 0)
            {
                validationProblem += "account id cannot have a value of zero or less,";
            }
        }
        return accountId;
    }
}

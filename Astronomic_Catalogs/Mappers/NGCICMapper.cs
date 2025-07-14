using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.ViewModels;

namespace Astronomic_Catalogs.Mappers;

public static class NGCICMapper
{
    public static NGCICViewModel ToViewModel(this NGCICOpendatasoft src)
    {
        return new NGCICViewModel
        {
            Id = src.Id,
            NGC_IC = src.NGC_IC,
            Name = src.Name,
            SubObject = src.SubObject,
            Messier = src.Messier is > 0 ? $"M{src.Messier}" : null,
            Name_UK = src.Name_UK,
            Comment = src.Comment,
            OtherNames = src.OtherNames,
            NGC = src.NGC,
            IC = src.IC,
            LimitAngDiameter = src.LimitAngDiameter,
            AngDiameter = src.AngDiameter?.ToString(),
            ObjectTypeAbrev = src.ObjectTypeAbrev,
            ObjectType = src.ObjectType,
            ObjectTypeFull = src.ObjectTypeFull,
            SourceType = src.SourceType,

            RA = src.RA,
            RightAscension = src.RightAscension,
            RightAscensionH = src.RightAscensionH,
            RightAscensionM = src.RightAscensionM,
            RightAscensionS = src.RightAscensionS,

            DEC = src.DEC,
            Declination = src.Declination,
            NS = src.NS,
            DeclinationD = src.DeclinationD,
            DeclinationM = src.DeclinationM,
            DeclinationS = src.DeclinationS,

            Constellation = src.Constellation,
            MajorAxis = src.MajorAxis,
            MinorAxis = src.MinorAxis,
            PositionAngle = src.PositionAngle,

            AppMag = src.AppMag,
            AppMagFlag = src.AppMagFlag,
            BMag = src.BMag,
            VMag = src.VMag,
            JMag = src.JMag,
            HMag = src.HMag,
            KMag = src.KMag,

            SurfaceBrightness = src.SurfaceBrightness,
            HubbleOnlyGalaxies = src.HubbleOnlyGalaxies,
            CstarUMag = src.CstarUMag,
            CstarBMag = src.CstarBMag,
            CstarVMag = src.CstarVMag,
            CstarNames = src.CstarNames,
            CommonNames = src.CommonNames,
            NedNotes = src.NedNotes,
            OpenngcNotes = src.OpenngcNotes,
            Image = src.Image,

            SourceTable = src.SourceTable,
            RowOnPage = src.RowOnPage
        };
    }

    public static List<NGCICViewModel> ToViewModelList(this List<NGCICOpendatasoft>? source)
    {
        return source?.Select(x => x.ToViewModel()).ToList() ?? new();
    }

    public static NGCICOpendatasoft ToEntity(this NGCICViewModel viewModel)
    {
        return new NGCICOpendatasoft
        {
            Id = viewModel.Id,
            NGC_IC = viewModel.NGC_IC,
            Name = viewModel.Name,
            SubObject = viewModel.SubObject,
            Messier = TryParseMessier(viewModel.Messier),
            Name_UK = viewModel.Name_UK,
            Comment = viewModel.Comment,
            OtherNames = viewModel.OtherNames,
            NGC = viewModel.NGC,
            IC = viewModel.IC,
            LimitAngDiameter = viewModel.LimitAngDiameter,
            AngDiameter = TryParseDouble(viewModel.AngDiameter),
            ObjectTypeAbrev = viewModel.ObjectTypeAbrev,
            ObjectType = viewModel.ObjectType,
            ObjectTypeFull = viewModel.ObjectTypeFull,
            SourceType = viewModel.SourceType,

            RA = viewModel.RA,
            RightAscension = viewModel.RightAscension,
            RightAscensionH = viewModel.RightAscensionH,
            RightAscensionM = viewModel.RightAscensionM,
            RightAscensionS = viewModel.RightAscensionS,

            DEC = viewModel.DEC,
            Declination = viewModel.Declination,
            NS = viewModel.NS,
            DeclinationD = viewModel.DeclinationD,
            DeclinationM = viewModel.DeclinationM,
            DeclinationS = viewModel.DeclinationS,

            Constellation = viewModel.Constellation,
            MajorAxis = viewModel.MajorAxis,
            MinorAxis = viewModel.MinorAxis,
            PositionAngle = viewModel.PositionAngle,

            AppMag = viewModel.AppMag,
            AppMagFlag = viewModel.AppMagFlag,
            BMag = viewModel.BMag,
            VMag = viewModel.VMag,
            JMag = viewModel.JMag,
            HMag = viewModel.HMag,
            KMag = viewModel.KMag,

            SurfaceBrightness = viewModel.SurfaceBrightness,
            HubbleOnlyGalaxies = viewModel.HubbleOnlyGalaxies,
            CstarUMag = viewModel.CstarUMag,
            CstarBMag = viewModel.CstarBMag,
            CstarVMag = viewModel.CstarVMag,
            CstarNames = viewModel.CstarNames,
            CommonNames = viewModel.CommonNames,
            NedNotes = viewModel.NedNotes,
            OpenngcNotes = viewModel.OpenngcNotes,
            Image = viewModel.Image,

            SourceTable = viewModel.SourceTable,
            RowOnPage = viewModel.RowOnPage
        };
    }

    private static int? TryParseMessier(string? messier)
    {
        if (string.IsNullOrWhiteSpace(messier)) return null;
        var digits = messier.Replace("M", "").Trim();
        return int.TryParse(digits, out var number) ? number : null;
    }

    private static double? TryParseDouble(string? value)
    {
        return double.TryParse(value, out var result) ? result : null;
    }

}


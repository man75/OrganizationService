namespace InterventionService.Domain.Enums;


public enum InterventionType
{
    Maintenance = 1,   // Vidange, révision, entretien périodique
    Repair = 2,        // Réparation mécanique, freinage, embrayage
    Diagnostic = 3,    // Diagnostic électronique / recherche de panne
    Inspection = 4,    // Contrôle, check-up, expertise
    Bodywork = 5,      // Carrosserie, peinture
    TireService = 6,   // Pneus, équilibrage, parallélisme
    Electrical = 7     // Batterie, alternateur, éclairage
}
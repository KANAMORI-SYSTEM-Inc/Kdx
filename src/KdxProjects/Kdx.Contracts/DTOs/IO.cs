namespace Kdx.Contracts.DTOs
{
    public class IO
    {
        public string Address { get; set; } = string.Empty; // �A�h���X

        public int PlcId { get; set; }

        // Properties from KdxDesigner version
        public string? IOText { get; set; }

        public string? XComment { get; set; } // X_Comment

        public string? YComment { get; set; } // Y_Comment

        public string? FComment { get; set; } // F_Comment

        public string? IOName { get; set; } // �A�N�`���G�[�^����

        public string? IOExplanation { get; set; } // �A�N�`���G�[�^����

        public string? IOSpot { get; set; } // ���j�b�g�ݒu�ꏊ

        public string? UnitName { get; set; } // ���j�b�g����

        public string? System { get; set; } // �n��

        public string? StationNumber { get; set; } // �ǔ�

        public string? IONameNaked { get; set; }

        public string? LinkDevice { get; set; }

        public int? IOType { get; set; }

        public bool? IsInverted { get; set; }

        public bool? IsEnabled { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DebitRequestTableDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CardId { get; set; }
        public string CardNumber { get; set; }
        public string CardDescription { get; set; }
        public string MerchantProductName { get; set; }
        public string MerchantName { get; set; }
        public Guid MerchantId { get; set; }
        public long ReferenceNumber { get; set; }
        public IList<DebitRequestReferenceTableDto> References { get; set; }
        public DateTime CreationDate { get; set; }
        public DebitRequestTypeDto Type { get; set; }
        public DebitRequestStateDto State { get; set; }
        public string DebitImageUrl { get; set; }
    }

    public class DebitRequestReferenceTableDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
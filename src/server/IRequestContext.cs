﻿namespace Domain0.Service
{
    public interface IRequestContext
    {
        int UserId { get; }

        string ClientHost { get; }
    }
}

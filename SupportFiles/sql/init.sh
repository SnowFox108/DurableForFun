#!/bin/bash

echo "Kerberos app started at $(date)"

while true; do
    echo "Refreshing Kerberos ticket at $(date)..."
    klist

    result=$?

    if [[ $result -eq 0 ]]; then
        echo "Success. Sleeping for $REKINIT_PERIOD seconds..."
    else
        echo "Failed to refresh Kerberos ticket. Result = $result"
        exit 1
    fi

    sleep "$REKINIT_PERIOD"
done
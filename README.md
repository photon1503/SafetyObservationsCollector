# SafetyObservationsCollector

** This project is work in progress **

This application can combine a SafetyMonitor and a ObservingConditions ASCOM driver into a single safe/unsafe decision.

* Safe/Unsafe state from SafetyMonitor is taken 

* Every sensor item from the ObservingConditions driver can be taken into evalution.

* The output from both drivers will finally generate a safe/unsafe state.

* A Boltwood Weather compatible file will be written, one with decimal point and another with commas.

** Notes

- Negative wind speeds will be converted to their absolute value
- The condition fields for Clouds, Wind, Rain and Daylight are not updated in the Boltwood file. They will default to "0" (unkown)


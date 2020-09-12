# SafetyObservationsCollector


## This project is work in progress 

This application can combine a SafetyMonitor and a ObservingConditions ASCOM driver into a single safe/unsafe decision.

## Usage

* Select your ObservationsCondition ASCOM driver. Can also be a hub.
* Select your SafteyMonitor ASCOM driver. This also can be a hub.
* Select the path were the Boltwood file will be written.
* Click "start"

## Process
* The state of the SafetyMonitor and ObservationsConditions drivers are queried every second
  
* Safe/Unsafe state from SafetyMonitor is taken as-is

* Every sensor item from the ObservingConditions driver can be taken into evalution. Simply click on the checkbox and update the min / max limits to your needs.
  If the sensor is *not checked*, it will be evaluated as "safe"

* The output from both drivers will finally generate a safe/unsafe state.

* A Boltwood Weather compatible file will be written, one with decimal point and another with commas.


## Screenshot

![screen1](screenshots/2020-09-12%2017_47_36-.png)

### Comments

- Negative wind speeds will be converted to their absolute value
- The condition fields for Clouds, Wind, Rain and Daylight are not updated in the Boltwood file. They will default to "0" (unkown)
#include <LiquidCrystal_I2C.h>
#include <Servo.h>

#define stop1Pin 8
#define stop2Pin 7
#define trigPin 9
#define echoPin 10
#define redPin 2
#define greenPin 3
#define bluePin 4

Servo servo1;
Servo servo2;

int pos1 = 0;
int pos2 = 0;
bool door = false;
bool isOpen = true;
bool hasPassedStart = false;
bool isDone = false;

int currentStation = 1;
int startStation = 2;
int endStation = 1;

bool atStart;
bool atEnd;
bool hasReceived;
bool startAtEnd;

LiquidCrystal_I2C lcd(0x27, 16, 2);

String message;

double distance = 10;
double duration;

void setup() {
  Serial.begin(9600);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  servo1.attach(stop1Pin);
  lcd.begin();
  lcd.backlight();
  pinMode(redPin, OUTPUT);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);
}

void Distance() {
  digitalWrite(trigPin, LOW);
  delay(2);
  digitalWrite(trigPin, HIGH);
  delay(10);
  digitalWrite(trigPin, LOW);
  duration = pulseIn(echoPin, HIGH);
  distance = duration * 0.034 / 2;
}

void RGB(int redValue, int greenValue, int blueValue){
  analogWrite(redPin, redValue);
  analogWrite(greenPin, greenValue);
  analogWrite(bluePin, blueValue);
}

void showScreen() {
  if(currentStation == startStation || currentStation == endStation && hasPassedStart){
    lcd.clear();
    lcd.setCursor(1, 0);
    lcd.print("Train Location:");
    lcd.setCursor(1,1);
    lcd.print(currentStation);
  }
  if(currentStation != endStation || startAtEnd || currentStation == endStation && !hasPassedStart && !isDone){
    delay(1000);
    lcd.clear();
    lcd.setCursor(1, 0);
    lcd.print("Train Location:");
    if(currentStation == 3){ 
      lcd.setCursor(1,1);
      char info[64];
      sprintf(info, "%d -> 1", currentStation);
      lcd.print(info);
    }
    else{
      lcd.setCursor(1,1);
      char info[64];
      sprintf(info, "%d -> %d", currentStation, currentStation + 1);
      lcd.print(info);
    }
  }
}

void stopTrain(){
  if(door && isOpen){  
    servo2.attach(stop2Pin);
    servo2.write(10);
    for (pos1 = 0; pos1 <= 90; pos1 ++) { 
    servo1.write(pos1);   
    delay(5);             
    }
    servo2.detach();
    isOpen = false;
    RGB(255, 0, 0);
    delay(100);
  }
  else if(!door && !isOpen){  
    servo2.attach(stop2Pin);
    servo2.write(10);
    delay(100);
    for (pos1 = 90; pos1 >= 0; pos1 --) {
    servo1.write(pos1);  
    delay(5);                
    }
    servo2.detach();
    isOpen = true;
    RGB(0, 255, 0);
    if(currentStation == startStation){
      Serial.println("Train has left the station"); 
    }
    delay(100);
  }   
}

void moveTrain(){
  if(currentStation == startStation  && !atStart){
    atStart = true;
    atEnd = false;
    hasPassedStart = true;

    if(startStation != endStation){
      door = true;
      stopTrain();
      delay(1000);
      door = false;
      stopTrain();
    }
  }

  if(atStart && !atEnd){
    if(currentStation == endStation){
      atEnd = true;
      atStart = false;
      hasReceived = false;
      
      door = true;
      stopTrain();
      Serial.println("Train has arrived"); 
      hasPassedStart = false;
      hasReceived = false;
    }
  }
}

void checkMovement(){
  if(distance < 10){
    currentStation++;
    if(currentStation == 4){
      currentStation = 1;
    }
    startAtEnd = false;
    Serial.println(currentStation);
    delay(150);
    showScreen();
  }
}

void loop() {
  if(hasReceived){
    Distance();
    checkMovement(); 
    moveTrain();
  }
  
  if (Serial.available() > 0)
  {
    char received = Serial.read();
    
    if(received == ','){
      char info[64];
      sprintf(info, "CurrentStation: %d", currentStation);
      Serial.println(info);
      delay(150);
      Serial.println("StartStation: " + message);
      delay(100);
      startStation = message.toInt();
      message = "";
      isDone = false;
    }
    else if(message == "-1"){
      Serial.println("Done");
      startStation = currentStation;
      endStation = currentStation;
      message = "";
      isDone = true;
      showScreen();
    }
    else if (received == '\n' && !isDone)
    {
      Serial.println("EndStation: " + message);
      delay(100);
      endStation = message.toInt();
      if(endStation == currentStation && endStation != startStation){
        startAtEnd = true;
      }
      else{
        startAtEnd = false;
      }
      if(currentStation != startStation){
        door = false;
        stopTrain();
      }
      hasReceived = true;
      message = "";
      showScreen();
    }
    else 
    {
      message += received;
    }
  }
}
